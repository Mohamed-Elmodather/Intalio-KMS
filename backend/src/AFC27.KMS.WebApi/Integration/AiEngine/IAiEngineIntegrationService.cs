using System;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.AiEngine.Models;

namespace AFC27.KMS.WebApi.Integration.AiEngine;

/// <summary>
/// Interface for AI Engine integration service
/// </summary>
public interface IAiEngineIntegrationService : IExternalServiceClient
{
    /// <summary>
    /// Classifies a document into categories
    /// </summary>
    Task<ServiceResponse<DocumentClassificationResult>> ClassifyDocumentAsync(
        DocumentClassificationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a summary of content
    /// </summary>
    Task<ServiceResponse<SummarizationResult>> SummarizeContentAsync(
        SummarizationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs semantic search across documents
    /// </summary>
    Task<ServiceResponse<SemanticSearchResult>> SemanticSearchAsync(
        SemanticSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content recommendations
    /// </summary>
    Task<ServiceResponse<RecommendationResult>> GetRecommendationsAsync(
        RecommendationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts entities from content
    /// </summary>
    Task<ServiceResponse<EntityExtractionResult>> ExtractEntitiesAsync(
        EntityExtractionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes document quality
    /// </summary>
    Task<ServiceResponse<QualityAnalysisResult>> AnalyzeQualityAsync(
        QualityAnalysisRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes a document for semantic search
    /// </summary>
    Task<ServiceResponse<bool>> IndexDocumentAsync(
        Guid documentId,
        string content,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a document from the search index
    /// </summary>
    Task<ServiceResponse<bool>> RemoveFromIndexAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates webhook signature
    /// </summary>
    bool ValidateWebhookSignature(AiEngineWebhookPayload payload, string secret);
}
