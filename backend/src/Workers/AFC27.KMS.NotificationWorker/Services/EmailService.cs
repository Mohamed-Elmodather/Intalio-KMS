using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace AFC27.KMS.NotificationWorker.Services;

/// <summary>
/// Service for sending email notifications using MailKit.
/// </summary>
public class EmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailOptions> options,
        ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email notification.
    /// </summary>
    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));

            if (cc != null)
            {
                foreach (var ccAddress in cc)
                {
                    message.Cc.Add(MailboxAddress.Parse(ccAddress));
                }
            }

            if (bcc != null)
            {
                foreach (var bccAddress in bcc)
                {
                    message.Bcc.Add(MailboxAddress.Parse(bccAddress));
                }
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
                bodyBuilder.TextBody = StripHtml(body);
            }
            else
            {
                bodyBuilder.TextBody = body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            if (_options.UseMock)
            {
                _logger.LogInformation(
                    "MOCK: Would send email to {To}, subject: {Subject}",
                    to, subject);
                return true;
            }

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _options.SmtpHost,
                _options.SmtpPort,
                _options.UseSsl,
                cancellationToken);

            if (!string.IsNullOrEmpty(_options.Username))
            {
                await client.AuthenticateAsync(
                    _options.Username,
                    _options.Password,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent to {To}, subject: {Subject}", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }

    /// <summary>
    /// Sends an email using a template.
    /// </summary>
    public async Task<bool> SendTemplatedEmailAsync(
        string to,
        string templateName,
        Dictionary<string, string> templateData,
        CancellationToken cancellationToken = default)
    {
        var template = await LoadTemplateAsync(templateName);
        if (template == null)
        {
            _logger.LogError("Email template not found: {TemplateName}", templateName);
            return false;
        }

        var subject = ReplaceTokens(template.Subject, templateData);
        var body = ReplaceTokens(template.Body, templateData);

        return await SendEmailAsync(to, subject, body, true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sends emails to multiple recipients.
    /// </summary>
    public async Task<int> SendBulkEmailAsync(
        IEnumerable<string> recipients,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        var successCount = 0;
        var tasks = recipients.Select(async recipient =>
        {
            var success = await SendEmailAsync(recipient, subject, body, isHtml, cancellationToken: cancellationToken);
            if (success)
            {
                Interlocked.Increment(ref successCount);
            }
        });

        await Task.WhenAll(tasks);

        _logger.LogInformation(
            "Bulk email sent to {SuccessCount}/{TotalCount} recipients",
            successCount,
            recipients.Count());

        return successCount;
    }

    private Task<EmailTemplate?> LoadTemplateAsync(string templateName)
    {
        // In production, load templates from database or file system
        // For now, return hardcoded templates
        var templates = new Dictionary<string, EmailTemplate>
        {
            ["welcome"] = new EmailTemplate
            {
                Subject = "Welcome to AFC Asian Cup 2027 KMS, {{UserName}}!",
                Body = @"
                    <h1>Welcome, {{UserName}}!</h1>
                    <p>Your account has been created for the AFC Asian Cup 2027 Knowledge Management System.</p>
                    <p>Click <a href='{{LoginUrl}}'>here</a> to get started.</p>
                "
            },
            ["document-shared"] = new EmailTemplate
            {
                Subject = "{{SenderName}} shared a document with you",
                Body = @"
                    <h2>Document Shared</h2>
                    <p>{{SenderName}} has shared the document <strong>{{DocumentName}}</strong> with you.</p>
                    <p><a href='{{DocumentUrl}}'>View Document</a></p>
                "
            },
            ["task-assigned"] = new EmailTemplate
            {
                Subject = "New task assigned: {{TaskTitle}}",
                Body = @"
                    <h2>Task Assigned</h2>
                    <p>You have been assigned a new task:</p>
                    <p><strong>{{TaskTitle}}</strong></p>
                    <p>{{TaskDescription}}</p>
                    <p>Due: {{DueDate}}</p>
                    <p><a href='{{TaskUrl}}'>View Task</a></p>
                "
            },
            ["approval-request"] = new EmailTemplate
            {
                Subject = "Approval Required: {{ItemName}}",
                Body = @"
                    <h2>Approval Request</h2>
                    <p>{{RequesterName}} is requesting your approval for:</p>
                    <p><strong>{{ItemName}}</strong></p>
                    <p><a href='{{ApprovalUrl}}'>Review and Approve</a></p>
                "
            }
        };

        templates.TryGetValue(templateName, out var template);
        return Task.FromResult(template);
    }

    private static string ReplaceTokens(string template, Dictionary<string, string> data)
    {
        foreach (var kvp in data)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }
        return template;
    }

    private static string StripHtml(string html)
    {
        // Simple HTML stripping - in production use HtmlAgilityPack
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        return System.Net.WebUtility.HtmlDecode(text.Trim());
    }
}

/// <summary>
/// Email configuration options.
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string FromEmail { get; set; } = "noreply@afc27-kms.com";
    public string FromName { get; set; } = "AFC Asian Cup 2027 KMS";
    public bool UseMock { get; set; } = true;
}

/// <summary>
/// Email template structure.
/// </summary>
public class EmailTemplate
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
