using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Features.Templates.Models;

namespace AFC27.KMS.WebApi.Features.Templates.Services;

/// <summary>
/// Interface for document template management service
/// </summary>
public interface IDocumentTemplateService
{
    /// <summary>
    /// Creates a new document template
    /// </summary>
    Task<DocumentTemplate> CreateTemplateAsync(
        CreateTemplateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing template
    /// </summary>
    Task<DocumentTemplate> UpdateTemplateAsync(
        Guid templateId,
        CreateTemplateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a template by ID
    /// </summary>
    Task<DocumentTemplate?> GetTemplateAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets templates with optional filtering
    /// </summary>
    Task<(List<TemplateSummaryDto> Templates, int TotalCount)> GetTemplatesAsync(
        string? category = null,
        TemplateType? type = null,
        TemplateStatus? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets templates by category
    /// </summary>
    Task<List<TemplateSummaryDto>> GetTemplatesByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a template
    /// </summary>
    Task<TemplateValidationResult> ValidateTemplateAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates template content
    /// </summary>
    TemplateValidationResult ValidateTemplateContent(
        string content,
        TemplateType type);

    /// <summary>
    /// Generates a document from a template
    /// </summary>
    Task<GeneratedDocumentResponse> GenerateDocumentAsync(
        GenerateDocumentRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets preview of generated document
    /// </summary>
    Task<byte[]> GetDocumentPreviewAsync(
        GenerateDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a template (changes status to Active)
    /// </summary>
    Task<DocumentTemplate> PublishTemplateAsync(
        Guid templateId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deprecates a template
    /// </summary>
    Task<DocumentTemplate> DeprecateTemplateAsync(
        Guid templateId,
        Guid userId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new version of a template
    /// </summary>
    Task<DocumentTemplate> CreateNewVersionAsync(
        Guid templateId,
        CreateTemplateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets template version history
    /// </summary>
    Task<List<TemplateSummaryDto>> GetTemplateVersionsAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a template (soft delete)
    /// </summary>
    Task DeleteTemplateAsync(
        Guid templateId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets template usage statistics
    /// </summary>
    Task<TemplateUsageStats> GetTemplateUsageStatsAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all template categories
    /// </summary>
    Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a template file (Word, Excel, PowerPoint)
    /// </summary>
    Task<DocumentTemplate> UploadTemplateFileAsync(
        string name,
        string category,
        Stream fileStream,
        string fileName,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts variables from template content
    /// </summary>
    List<TemplateVariable> ExtractVariables(string content, TemplateType type);
}
