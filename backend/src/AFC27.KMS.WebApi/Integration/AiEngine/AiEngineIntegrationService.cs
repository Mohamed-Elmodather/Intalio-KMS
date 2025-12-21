using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.AiEngine.Models;

namespace AFC27.KMS.WebApi.Integration.AiEngine;

/// <summary>
/// Implementation of AI Engine integration service
/// </summary>
public class AiEngineIntegrationService : ExternalServiceClientBase, IAiEngineIntegrationService
{
    private readonly AiEngineSettings _settings;

    public override string ServiceName => "AiEngine";

    public AiEngineIntegrationService(
        HttpClient httpClient,
        ILogger<AiEngineIntegrationService> logger,
        IOptions<IntegrationSettings> settings)
        : base(httpClient, logger, settings.Value.AiEngine)
    {
        _settings = settings.Value.AiEngine;
    }

    public async Task<ServiceResponse<DocumentClassificationResult>> ClassifyDocumentAsync(
        DocumentClassificationRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Classifying document {DocumentId}",
            request.DocumentId);

        return await PostAsync<DocumentClassificationRequest, DocumentClassificationResult>(
            "ai/classify",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<SummarizationResult>> SummarizeContentAsync(
        SummarizationRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Summarizing content of length {Length}",
            request.Content.Length);

        return await PostAsync<SummarizationRequest, SummarizationResult>(
            "ai/summarize",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<SemanticSearchResult>> SemanticSearchAsync(
        SemanticSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Performing semantic search for query: {Query}",
            request.Query);

        return await PostAsync<SemanticSearchRequest, SemanticSearchResult>(
            "ai/search",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<RecommendationResult>> GetRecommendationsAsync(
        RecommendationRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Getting {Type} recommendations for user {UserId}",
            request.Type, request.UserId);

        return await PostAsync<RecommendationRequest, RecommendationResult>(
            "ai/recommend",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<EntityExtractionResult>> ExtractEntitiesAsync(
        EntityExtractionRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Extracting entities from content");

        return await PostAsync<EntityExtractionRequest, EntityExtractionResult>(
            "ai/entities",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<QualityAnalysisResult>> AnalyzeQualityAsync(
        QualityAnalysisRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Analyzing quality for document {DocumentId}",
            request.DocumentId);

        return await PostAsync<QualityAnalysisRequest, QualityAnalysisResult>(
            "ai/quality",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<bool>> IndexDocumentAsync(
        Guid documentId,
        string content,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Indexing document {DocumentId} for semantic search",
            documentId);

        var request = new
        {
            DocumentId = documentId,
            Content = content,
            Metadata = metadata ?? new Dictionary<string, string>()
        };

        var response = await PostAsync<object, object>(
            "ai/index",
            request,
            cancellationToken);

        return new ServiceResponse<bool>
        {
            IsSuccess = response.IsSuccess,
            Data = response.IsSuccess,
            ErrorMessage = response.ErrorMessage,
            ErrorCode = response.ErrorCode,
            HttpStatusCode = response.HttpStatusCode
        };
    }

    public async Task<ServiceResponse<bool>> RemoveFromIndexAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Removing document {DocumentId} from search index",
            documentId);

        var response = await DeleteAsync<object>(
            $"ai/index/{documentId}",
            cancellationToken);

        return new ServiceResponse<bool>
        {
            IsSuccess = response.IsSuccess,
            Data = response.IsSuccess,
            ErrorMessage = response.ErrorMessage,
            ErrorCode = response.ErrorCode,
            HttpStatusCode = response.HttpStatusCode
        };
    }

    public bool ValidateWebhookSignature(AiEngineWebhookPayload payload, string secret)
    {
        if (string.IsNullOrEmpty(payload.Signature) || string.IsNullOrEmpty(secret))
            return false;

        var dataToSign = $"{payload.JobId}:{payload.JobType}:{payload.Status}:{payload.Timestamp:O}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        var computedSignature = Convert.ToBase64String(hash);

        return payload.Signature == computedSignature;
    }
}
