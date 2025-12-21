using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AFC27.KMS.WebApi.Features.Templates.Models;

namespace AFC27.KMS.WebApi.Features.Templates.Services;

/// <summary>
/// Implementation of document template management service
/// </summary>
public class DocumentTemplateService : IDocumentTemplateService
{
    private readonly ILogger<DocumentTemplateService> _logger;

    // In-memory storage for demo - replace with repository in production
    private static readonly List<DocumentTemplate> _templates = new();
    private static readonly List<DocumentGeneration> _generations = new();

    // Variable pattern: {{variableName}} or ${variableName} or [variableName]
    private static readonly Regex VariablePattern = new(
        @"(?:\{\{([^}]+)\}\}|\$\{([^}]+)\}|\[([^\]]+)\])",
        RegexOptions.Compiled);

    public DocumentTemplateService(ILogger<DocumentTemplateService> logger)
    {
        _logger = logger;

        // Seed with demo templates if empty
        if (!_templates.Any())
        {
            SeedDemoTemplates();
        }
    }

    public async Task<DocumentTemplate> CreateTemplateAsync(
        CreateTemplateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var template = new DocumentTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Type = request.Type,
            ContentType = request.ContentType ?? GetDefaultContentType(request.Type),
            TemplateContent = request.TemplateContent ?? string.Empty,
            Variables = request.Variables ?? new List<TemplateVariable>(),
            Metadata = request.Metadata ?? new TemplateMetadata(),
            Status = TemplateStatus.Draft,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        // Auto-extract variables if none provided
        if (!template.Variables.Any() && !string.IsNullOrEmpty(template.TemplateContent))
        {
            template.Variables = ExtractVariables(template.TemplateContent, template.Type);
        }

        _templates.Add(template);

        _logger.LogInformation(
            "Created template {TemplateId}: {TemplateName}",
            template.Id, template.Name);

        return await Task.FromResult(template);
    }

    public async Task<DocumentTemplate> UpdateTemplateAsync(
        Guid templateId,
        CreateTemplateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var template = _templates.FirstOrDefault(t => t.Id == templateId && !t.IsDeleted);
        if (template == null)
            throw new KeyNotFoundException($"Template {templateId} not found");

        template.Name = request.Name;
        template.Description = request.Description;
        template.Category = request.Category;
        template.Type = request.Type;
        template.ContentType = request.ContentType ?? template.ContentType;
        template.TemplateContent = request.TemplateContent ?? template.TemplateContent;
        template.Variables = request.Variables ?? template.Variables;
        template.Metadata = request.Metadata ?? template.Metadata;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = userId;

        _logger.LogInformation(
            "Updated template {TemplateId}: {TemplateName}",
            template.Id, template.Name);

        return await Task.FromResult(template);
    }

    public Task<DocumentTemplate?> GetTemplateAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var template = _templates.FirstOrDefault(t => t.Id == templateId && !t.IsDeleted);
        return Task.FromResult(template);
    }

    public Task<(List<TemplateSummaryDto> Templates, int TotalCount)> GetTemplatesAsync(
        string? category = null,
        TemplateType? type = null,
        TemplateStatus? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _templates.Where(t => !t.IsDeleted);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(t =>
                t.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

        var totalCount = query.Count();

        var templates = query
            .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TemplateSummaryDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                Type = t.Type,
                Status = t.Status,
                Version = t.Version,
                VariableCount = t.Variables.Count,
                UsageCount = _generations.Count(g => g.TemplateId == t.Id),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToList();

        return Task.FromResult((templates, totalCount));
    }

    public Task<List<TemplateSummaryDto>> GetTemplatesByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        var templates = _templates
            .Where(t => !t.IsDeleted && t.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .Select(t => new TemplateSummaryDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                Type = t.Type,
                Status = t.Status,
                Version = t.Version,
                VariableCount = t.Variables.Count,
                CreatedAt = t.CreatedAt
            })
            .ToList();

        return Task.FromResult(templates);
    }

    public Task<TemplateValidationResult> ValidateTemplateAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var template = _templates.FirstOrDefault(t => t.Id == templateId && !t.IsDeleted);
        if (template == null)
        {
            return Task.FromResult(new TemplateValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "Template not found" }
            });
        }

        return Task.FromResult(ValidateTemplateContent(template.TemplateContent, template.Type));
    }

    public TemplateValidationResult ValidateTemplateContent(string content, TemplateType type)
    {
        var result = new TemplateValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(content))
        {
            result.IsValid = false;
            result.Errors.Add("Template content is empty");
            return result;
        }

        // Extract and validate variables
        var detectedVariables = new HashSet<string>();
        foreach (Match match in VariablePattern.Matches(content))
        {
            var varName = match.Groups[1].Success ? match.Groups[1].Value :
                         match.Groups[2].Success ? match.Groups[2].Value :
                         match.Groups[3].Value;
            detectedVariables.Add(varName.Trim());
        }

        result.DetectedVariables = detectedVariables.ToList();

        // Check for invalid variable names
        foreach (var variable in detectedVariables)
        {
            if (variable.Contains(" "))
            {
                result.Warnings.Add($"Variable '{variable}' contains spaces - consider using camelCase or underscores");
            }

            if (!Regex.IsMatch(variable, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                result.Warnings.Add($"Variable '{variable}' has an unconventional name format");
            }
        }

        // Type-specific validation
        switch (type)
        {
            case TemplateType.HtmlEmail:
                if (!content.Contains("<html", StringComparison.OrdinalIgnoreCase) &&
                    !content.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add("HTML template should include proper HTML structure");
                }
                break;

            case TemplateType.Markdown:
                // Basic markdown validation - check for common issues
                break;
        }

        return result;
    }

    public async Task<GeneratedDocumentResponse> GenerateDocumentAsync(
        GenerateDocumentRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync(request.TemplateId, cancellationToken);
        if (template == null)
            throw new KeyNotFoundException($"Template {request.TemplateId} not found");

        // Validate required variables
        foreach (var variable in template.Variables.Where(v => v.IsRequired))
        {
            if (!request.VariableValues.ContainsKey(variable.Name))
            {
                throw new ArgumentException($"Required variable '{variable.Name}' is missing");
            }
        }

        // Generate the document content
        var generatedContent = SubstituteVariables(template.TemplateContent, request.VariableValues);

        // In production, this would use a document generation library
        // (e.g., Aspose, DocX, iTextSharp) to create the actual document
        var documentId = Guid.NewGuid();
        var fileName = request.OutputFileName ?? $"{template.Name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.OutputFormat}";

        // Track the generation
        _generations.Add(new DocumentGeneration
        {
            Id = documentId,
            TemplateId = template.Id,
            UserId = userId,
            GeneratedAt = DateTime.UtcNow,
            OutputFormat = request.OutputFormat
        });

        _logger.LogInformation(
            "Generated document {DocumentId} from template {TemplateId}",
            documentId, template.Id);

        return new GeneratedDocumentResponse
        {
            DocumentId = documentId,
            FileName = fileName,
            ContentType = GetContentTypeForFormat(request.OutputFormat),
            FileSizeBytes = generatedContent.Length * 2, // Approximate
            GeneratedAt = DateTime.UtcNow,
            TemplateId = template.Id,
            TemplateName = template.Name,
            TemplateVersion = template.Version
        };
    }

    public Task<byte[]> GetDocumentPreviewAsync(
        GenerateDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        // In production, generate a preview (first page as image or HTML)
        return Task.FromResult(Array.Empty<byte>());
    }

    public async Task<DocumentTemplate> PublishTemplateAsync(
        Guid templateId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync(templateId, cancellationToken);
        if (template == null)
            throw new KeyNotFoundException($"Template {templateId} not found");

        // Validate before publishing
        var validation = await ValidateTemplateAsync(templateId, cancellationToken);
        if (!validation.IsValid)
            throw new InvalidOperationException($"Template validation failed: {string.Join(", ", validation.Errors)}");

        template.Status = TemplateStatus.Active;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = userId;

        _logger.LogInformation("Published template {TemplateId}", templateId);

        return template;
    }

    public async Task<DocumentTemplate> DeprecateTemplateAsync(
        Guid templateId,
        Guid userId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync(templateId, cancellationToken);
        if (template == null)
            throw new KeyNotFoundException($"Template {templateId} not found");

        template.Status = TemplateStatus.Deprecated;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = userId;

        _logger.LogInformation("Deprecated template {TemplateId}: {Reason}", templateId, reason);

        return template;
    }

    public async Task<DocumentTemplate> CreateNewVersionAsync(
        Guid templateId,
        CreateTemplateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var parentTemplate = await GetTemplateAsync(templateId, cancellationToken);
        if (parentTemplate == null)
            throw new KeyNotFoundException($"Template {templateId} not found");

        // Deprecate the old version
        parentTemplate.Status = TemplateStatus.Deprecated;

        // Create new version
        var newTemplate = await CreateTemplateAsync(request, userId, cancellationToken);
        newTemplate.ParentTemplateId = templateId;
        newTemplate.Version = parentTemplate.Version + 1;

        _logger.LogInformation(
            "Created new version {Version} of template {TemplateId}",
            newTemplate.Version, templateId);

        return newTemplate;
    }

    public Task<List<TemplateSummaryDto>> GetTemplateVersionsAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var versions = new List<TemplateSummaryDto>();
        var currentId = templateId;

        while (currentId != Guid.Empty)
        {
            var template = _templates.FirstOrDefault(t => t.Id == currentId);
            if (template == null) break;

            versions.Add(new TemplateSummaryDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                Type = template.Type,
                Status = template.Status,
                Version = template.Version,
                CreatedAt = template.CreatedAt
            });

            currentId = template.ParentTemplateId ?? Guid.Empty;
        }

        return Task.FromResult(versions.OrderByDescending(v => v.Version).ToList());
    }

    public async Task DeleteTemplateAsync(
        Guid templateId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var template = await GetTemplateAsync(templateId, cancellationToken);
        if (template == null)
            throw new KeyNotFoundException($"Template {templateId} not found");

        template.IsDeleted = true;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = userId;

        _logger.LogInformation("Deleted template {TemplateId}", templateId);
    }

    public Task<TemplateUsageStats> GetTemplateUsageStatsAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        var generations = _generations.Where(g => g.TemplateId == templateId).ToList();

        var stats = new TemplateUsageStats
        {
            TemplateId = templateId,
            TemplateName = template?.Name ?? "Unknown",
            TotalGenerations = generations.Count,
            Last30DaysGenerations = generations.Count(g => g.GeneratedAt >= DateTime.UtcNow.AddDays(-30)),
            LastUsedAt = generations.MaxBy(g => g.GeneratedAt)?.GeneratedAt,
            GenerationsByMonth = generations
                .GroupBy(g => new { g.GeneratedAt.Year, g.GeneratedAt.Month })
                .Select(g => new GenerationByMonth
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.Month)
                .Take(12)
                .ToList()
        };

        return Task.FromResult(stats);
    }

    public Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = _templates
            .Where(t => !t.IsDeleted)
            .Select(t => t.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        return Task.FromResult(categories);
    }

    public Task<DocumentTemplate> UploadTemplateFileAsync(
        string name,
        string category,
        Stream fileStream,
        string fileName,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Determine template type from file extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var type = extension switch
        {
            ".docx" or ".doc" => TemplateType.WordDocument,
            ".xlsx" or ".xls" => TemplateType.ExcelSpreadsheet,
            ".pptx" or ".ppt" => TemplateType.PowerPointPresentation,
            ".pdf" => TemplateType.PdfForm,
            ".html" or ".htm" => TemplateType.HtmlEmail,
            ".md" => TemplateType.Markdown,
            _ => TemplateType.PlainText
        };

        // Read file content
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        var fileContent = memoryStream.ToArray();

        var template = new DocumentTemplate
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = $"Uploaded from {fileName}",
            Category = category,
            Type = type,
            ContentType = GetContentTypeForExtension(extension),
            TemplateFile = fileContent,
            TemplateFilePath = fileName,
            Variables = new List<TemplateVariable>(),
            Status = TemplateStatus.Draft,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _templates.Add(template);

        _logger.LogInformation(
            "Uploaded template file {TemplateId}: {FileName}",
            template.Id, fileName);

        return Task.FromResult(template);
    }

    public List<TemplateVariable> ExtractVariables(string content, TemplateType type)
    {
        var variables = new List<TemplateVariable>();
        var seen = new HashSet<string>();

        foreach (Match match in VariablePattern.Matches(content))
        {
            var varName = match.Groups[1].Success ? match.Groups[1].Value :
                         match.Groups[2].Success ? match.Groups[2].Value :
                         match.Groups[3].Value;

            varName = varName.Trim();
            if (seen.Contains(varName)) continue;
            seen.Add(varName);

            variables.Add(new TemplateVariable
            {
                Name = varName,
                DisplayName = FormatDisplayName(varName),
                Type = InferVariableType(varName),
                IsRequired = true
            });
        }

        return variables;
    }

    private string SubstituteVariables(string content, Dictionary<string, object> values)
    {
        foreach (var (key, value) in values)
        {
            var patterns = new[]
            {
                $"{{{{{key}}}}}",  // {{variable}}
                $"${{{key}}}",    // ${variable}
                $"[{key}]"        // [variable]
            };

            foreach (var pattern in patterns)
            {
                content = content.Replace(pattern, value?.ToString() ?? string.Empty);
            }
        }

        return content;
    }

    private static string GetDefaultContentType(TemplateType type) => type switch
    {
        TemplateType.WordDocument => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        TemplateType.ExcelSpreadsheet => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        TemplateType.PowerPointPresentation => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        TemplateType.PdfForm => "application/pdf",
        TemplateType.HtmlEmail => "text/html",
        TemplateType.Markdown => "text/markdown",
        _ => "text/plain"
    };

    private static string GetContentTypeForFormat(string format) => format.ToLowerInvariant() switch
    {
        "pdf" => "application/pdf",
        "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "html" => "text/html",
        _ => "application/octet-stream"
    };

    private static string GetContentTypeForExtension(string extension) => extension switch
    {
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ".doc" => "application/msword",
        ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ".xls" => "application/vnd.ms-excel",
        ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        ".ppt" => "application/vnd.ms-powerpoint",
        ".pdf" => "application/pdf",
        ".html" or ".htm" => "text/html",
        ".md" => "text/markdown",
        _ => "application/octet-stream"
    };

    private static string FormatDisplayName(string variableName)
    {
        // Convert camelCase or snake_case to Title Case
        var result = Regex.Replace(variableName, @"([a-z])([A-Z])", "$1 $2");
        result = result.Replace("_", " ");
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.ToLower());
    }

    private static VariableType InferVariableType(string variableName)
    {
        var nameLower = variableName.ToLowerInvariant();

        if (nameLower.Contains("date")) return VariableType.Date;
        if (nameLower.Contains("time")) return VariableType.DateTime;
        if (nameLower.Contains("email")) return VariableType.Email;
        if (nameLower.Contains("phone") || nameLower.Contains("mobile")) return VariableType.Phone;
        if (nameLower.Contains("url") || nameLower.Contains("link")) return VariableType.Url;
        if (nameLower.Contains("amount") || nameLower.Contains("price") || nameLower.Contains("cost")) return VariableType.Currency;
        if (nameLower.Contains("count") || nameLower.Contains("number") || nameLower.Contains("qty")) return VariableType.Number;
        if (nameLower.Contains("image") || nameLower.Contains("photo") || nameLower.Contains("logo")) return VariableType.Image;
        if (nameLower.Contains("signature")) return VariableType.Signature;
        if (nameLower.Contains("barcode")) return VariableType.Barcode;
        if (nameLower.Contains("qr") || nameLower.Contains("qrcode")) return VariableType.QrCode;

        return VariableType.Text;
    }

    private void SeedDemoTemplates()
    {
        var demoTemplates = new List<DocumentTemplate>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Official Letter Template",
                Description = "Standard official letter format for AFC 2027 communications",
                Category = "Official Communications",
                Type = TemplateType.WordDocument,
                Status = TemplateStatus.Active,
                Version = 1,
                TemplateContent = @"
                    <header>
                        {{organizationLogo}}
                        AFC Asian Cup 2027
                        {{date}}
                    </header>

                    To: {{recipientName}}
                    {{recipientTitle}}
                    {{recipientOrganization}}

                    Subject: {{subject}}

                    Dear {{salutation}},

                    {{bodyContent}}

                    Sincerely,

                    {{senderName}}
                    {{senderTitle}}
                    {{senderDepartment}}
                ",
                Variables = new List<TemplateVariable>
                {
                    new() { Name = "organizationLogo", DisplayName = "Organization Logo", Type = VariableType.Image, IsRequired = false },
                    new() { Name = "date", DisplayName = "Date", Type = VariableType.Date, IsRequired = true },
                    new() { Name = "recipientName", DisplayName = "Recipient Name", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "recipientTitle", DisplayName = "Recipient Title", Type = VariableType.Text, IsRequired = false },
                    new() { Name = "recipientOrganization", DisplayName = "Recipient Organization", Type = VariableType.Text, IsRequired = false },
                    new() { Name = "subject", DisplayName = "Subject", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "salutation", DisplayName = "Salutation", Type = VariableType.Text, IsRequired = true, DefaultValue = "Mr./Ms." },
                    new() { Name = "bodyContent", DisplayName = "Body Content", Type = VariableType.RichText, IsRequired = true },
                    new() { Name = "senderName", DisplayName = "Sender Name", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "senderTitle", DisplayName = "Sender Title", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "senderDepartment", DisplayName = "Sender Department", Type = VariableType.Text, IsRequired = false }
                },
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                CreatedBy = Guid.Empty
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Meeting Minutes Template",
                Description = "Standard template for committee and working group meeting minutes",
                Category = "Meeting Documents",
                Type = TemplateType.WordDocument,
                Status = TemplateStatus.Active,
                Version = 1,
                TemplateContent = @"
                    MEETING MINUTES

                    Meeting: {{meetingTitle}}
                    Date: {{meetingDate}}
                    Time: {{startTime}} - {{endTime}}
                    Location: {{location}}

                    ATTENDEES:
                    {{attendeesList}}

                    APOLOGIES:
                    {{apologies}}

                    AGENDA ITEMS:
                    {{agendaItems}}

                    DECISIONS:
                    {{decisions}}

                    ACTION ITEMS:
                    {{actionItems}}

                    NEXT MEETING:
                    Date: {{nextMeetingDate}}
                    Location: {{nextMeetingLocation}}

                    Minutes prepared by: {{preparedBy}}
                    Date: {{preparedDate}}
                ",
                Variables = new List<TemplateVariable>
                {
                    new() { Name = "meetingTitle", DisplayName = "Meeting Title", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "meetingDate", DisplayName = "Meeting Date", Type = VariableType.Date, IsRequired = true },
                    new() { Name = "startTime", DisplayName = "Start Time", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "endTime", DisplayName = "End Time", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "location", DisplayName = "Location", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "attendeesList", DisplayName = "Attendees List", Type = VariableType.List, IsRequired = true },
                    new() { Name = "agendaItems", DisplayName = "Agenda Items", Type = VariableType.Table, IsRequired = true },
                    new() { Name = "decisions", DisplayName = "Decisions", Type = VariableType.List, IsRequired = false },
                    new() { Name = "actionItems", DisplayName = "Action Items", Type = VariableType.Table, IsRequired = false }
                },
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                CreatedBy = Guid.Empty
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Accreditation Certificate",
                Description = "Certificate template for media and personnel accreditation",
                Category = "Accreditation",
                Type = TemplateType.PdfForm,
                Status = TemplateStatus.Active,
                Version = 1,
                TemplateContent = @"
                    AFC ASIAN CUP 2027
                    ACCREDITATION CERTIFICATE

                    This certifies that

                    {{fullName}}
                    {{organization}}

                    Has been granted {{accreditationType}} accreditation
                    for the AFC Asian Cup 2027 tournament.

                    Accreditation Number: {{accreditationNumber}}
                    Valid From: {{validFrom}}
                    Valid Until: {{validUntil}}

                    Access Zones: {{accessZones}}

                    {{qrCode}}

                    {{signature}}
                    Authorized Signature
                ",
                Variables = new List<TemplateVariable>
                {
                    new() { Name = "fullName", DisplayName = "Full Name", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "organization", DisplayName = "Organization", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "accreditationType", DisplayName = "Accreditation Type", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "accreditationNumber", DisplayName = "Accreditation Number", Type = VariableType.Text, IsRequired = true },
                    new() { Name = "validFrom", DisplayName = "Valid From", Type = VariableType.Date, IsRequired = true },
                    new() { Name = "validUntil", DisplayName = "Valid Until", Type = VariableType.Date, IsRequired = true },
                    new() { Name = "accessZones", DisplayName = "Access Zones", Type = VariableType.List, IsRequired = true },
                    new() { Name = "qrCode", DisplayName = "QR Code", Type = VariableType.QrCode, IsRequired = true },
                    new() { Name = "signature", DisplayName = "Signature", Type = VariableType.Signature, IsRequired = false }
                },
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                CreatedBy = Guid.Empty
            }
        };

        _templates.AddRange(demoTemplates);
    }

    private class DocumentGeneration
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public Guid UserId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string OutputFormat { get; set; } = string.Empty;
    }
}
