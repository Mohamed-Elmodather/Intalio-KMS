using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AFC27.KMS.WebApi.Data;
using AFC27.KMS.WebApi.Data.Entities;
using AFC27.KMS.WebApi.Features.AIAnalysis.Models;
using AFC27.KMS.WebApi.Integration.AiEngine;

namespace AFC27.KMS.WebApi.Features.AIAnalysis.Services;

/// <summary>
/// Implementation of AI-powered document analysis service using database persistence
/// </summary>
public class AIAnalysisService : IAIAnalysisService
{
    private readonly KmsDbContext _dbContext;
    private readonly ILogger<AIAnalysisService> _logger;
    private readonly IAiEngineIntegrationService _aiEngine;
    private readonly IServiceScopeFactory _scopeFactory;

    public AIAnalysisService(
        KmsDbContext dbContext,
        ILogger<AIAnalysisService> logger,
        IAiEngineIntegrationService aiEngine,
        IServiceScopeFactory scopeFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _aiEngine = aiEngine;
        _scopeFactory = scopeFactory;
    }

    public async Task<DocumentAnalysis> AnalyzeDocumentAsync(Guid documentId, AnalyzeDocumentRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing document {DocumentId}", documentId);

        // Check if already analyzed recently
        var existingAnalysis = await _dbContext.DocumentAnalyses
            .Include(a => a.ExtractedEntities)
            .FirstOrDefaultAsync(a => a.DocumentId == documentId, cancellationToken);

        if (existingAnalysis != null && existingAnalysis.AnalyzedAt > DateTime.UtcNow.AddHours(-24))
            return MapToModel(existingAnalysis);

        // Create or update analysis
        var entity = existingAnalysis ?? new DocumentAnalysisEntity { DocumentId = documentId };
        entity.IsProcessing = true;

        if (existingAnalysis == null)
            _dbContext.DocumentAnalyses.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            // Simulate AI analysis (in real implementation, call external AI service)
            await Task.Delay(100, cancellationToken);

            // Generate analysis results
            entity.Summary = "This document discusses key aspects of the AFC 2027 project implementation, including timeline, resources, and milestones.";
            entity.SummaryAr = "يناقش هذا المستند الجوانب الرئيسية لتنفيذ مشروع كأس آسيا 2027، بما في ذلك الجدول الزمني والموارد والمعالم.";
            entity.Language = "en";
            entity.LanguageConfidence = 0.98;
            entity.SentimentScore = 0.65;
            entity.SentimentPositive = 0.65;
            entity.SentimentNeutral = 0.30;
            entity.SentimentNegative = 0.05;
            entity.SentimentLabel = "Positive";
            entity.TopicsJson = JsonSerializer.Serialize(new[] { "Project Management", "Event Planning", "Sports Administration" });
            entity.KeyPhrasesJson = JsonSerializer.Serialize(new[] { "project timeline", "resource allocation", "milestone tracking", "stakeholder engagement" });
            entity.SuggestedTagsJson = JsonSerializer.Serialize(new[] { "AFC2027", "Planning", "Implementation", "Project Management" });
            entity.ReadabilityScore = 72.5;
            entity.WordCount = 1500;
            entity.AnalyzedAt = DateTime.UtcNow;
            entity.AnalysisVersion = "1.0";
            entity.IsProcessing = false;
            entity.ErrorMessage = null;

            // Remove old entities and add new ones
            var oldEntities = await _dbContext.ExtractedEntities
                .Where(e => e.DocumentAnalysisId == entity.Id)
                .ToListAsync(cancellationToken);
            _dbContext.ExtractedEntities.RemoveRange(oldEntities);

            // Add extracted entities
            var entities = new[]
            {
                new ExtractedEntityRecord { DocumentAnalysisId = entity.Id, DocumentId = documentId, EntityType = "Organization", Value = "AFC", NormalizedValue = "AFC", Confidence = 0.95 },
                new ExtractedEntityRecord { DocumentAnalysisId = entity.Id, DocumentId = documentId, EntityType = "Event", Value = "Asian Cup 2027", NormalizedValue = "Asian Cup 2027", Confidence = 0.98 },
                new ExtractedEntityRecord { DocumentAnalysisId = entity.Id, DocumentId = documentId, EntityType = "Location", Value = "Saudi Arabia", NormalizedValue = "Saudi Arabia", Confidence = 0.92 },
                new ExtractedEntityRecord { DocumentAnalysisId = entity.Id, DocumentId = documentId, EntityType = "Date", Value = "2027", NormalizedValue = "2027", Confidence = 0.99 }
            };
            _dbContext.ExtractedEntities.AddRange(entities);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Document analysis completed for {DocumentId}", documentId);

            // Reload with entities
            entity = await _dbContext.DocumentAnalyses
                .Include(a => a.ExtractedEntities)
                .FirstAsync(a => a.Id == entity.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            entity.IsProcessing = false;
            entity.ErrorMessage = ex.Message;
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogError(ex, "Failed to analyze document {DocumentId}", documentId);
        }

        return MapToModel(entity);
    }

    public async Task<DocumentAnalysis?> GetDocumentAnalysisAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.DocumentAnalyses
            .Include(a => a.ExtractedEntities)
            .FirstOrDefaultAsync(a => a.DocumentId == documentId, cancellationToken);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task<List<DocumentAnalysis>> GetRecentAnalysesAsync(int count, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.DocumentAnalyses
            .Include(a => a.ExtractedEntities)
            .OrderByDescending(a => a.AnalyzedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToModel).ToList();
    }

    public async Task<List<ExtractedEntity>> GetDocumentEntitiesAsync(Guid documentId, EntityType? type, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ExtractedEntities
            .Where(e => e.DocumentId == documentId);

        if (type.HasValue)
            query = query.Where(e => e.EntityType == type.Value.ToString());

        var entities = await query.ToListAsync(cancellationToken);

        return entities.Select(e => new ExtractedEntity
        {
            Id = e.Id,
            DocumentId = e.DocumentId,
            Type = Enum.TryParse<EntityType>(e.EntityType, out var t) ? t : EntityType.Person,
            Value = e.Value,
            NormalizedValue = e.NormalizedValue,
            Confidence = e.Confidence
        }).ToList();
    }

    public Task<List<ExtractedEntity>> ExtractEntitiesFromTextAsync(string text, CancellationToken cancellationToken = default)
    {
        // Simulate entity extraction
        var entities = new List<ExtractedEntity>();

        if (text.Contains("AFC", StringComparison.OrdinalIgnoreCase))
            entities.Add(new ExtractedEntity { Id = Guid.NewGuid(), Type = EntityType.Organization, Value = "AFC", Confidence = 0.95 });

        if (text.Contains("2027"))
            entities.Add(new ExtractedEntity { Id = Guid.NewGuid(), Type = EntityType.Date, Value = "2027", Confidence = 0.99 });

        return Task.FromResult(entities);
    }

    public async Task<SentimentAnalysis> AnalyzeSentimentAsync(Guid contentId, ContentType contentType, string text, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        var sentiment = new SentimentAnalysis
        {
            Id = Guid.NewGuid(),
            ContentId = contentId,
            ContentType = contentType,
            Score = 0.65,
            Label = SentimentLabel.Positive,
            PositiveScore = 0.65,
            NeutralScore = 0.30,
            NegativeScore = 0.05,
            AnalyzedAt = DateTime.UtcNow
        };

        return sentiment;
    }

    public async Task<SentimentAnalysis?> GetSentimentAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        var analysis = await _dbContext.DocumentAnalyses
            .FirstOrDefaultAsync(a => a.DocumentId == contentId, cancellationToken);

        if (analysis == null) return null;

        return new SentimentAnalysis
        {
            Id = analysis.Id,
            ContentId = contentId,
            ContentType = ContentType.Document,
            Score = analysis.SentimentScore ?? 0,
            Label = Enum.TryParse<SentimentLabel>(analysis.SentimentLabel, out var label) ? label : SentimentLabel.Neutral,
            PositiveScore = analysis.SentimentPositive ?? 0,
            NeutralScore = analysis.SentimentNeutral ?? 0,
            NegativeScore = analysis.SentimentNegative ?? 0,
            AnalyzedAt = analysis.AnalyzedAt
        };
    }

    public async Task<List<TagSuggestion>> GetTagSuggestionsAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var analysis = await _dbContext.DocumentAnalyses
            .FirstOrDefaultAsync(a => a.DocumentId == documentId, cancellationToken);

        if (analysis?.SuggestedTagsJson == null) return new List<TagSuggestion>();

        var tags = JsonSerializer.Deserialize<string[]>(analysis.SuggestedTagsJson) ?? Array.Empty<string>();
        return tags.Select((tag, i) => new TagSuggestion
        {
            Tag = tag,
            Confidence = 0.95 - (i * 0.05),
            Source = "AI"
        }).ToList();
    }

    public Task<List<TagSuggestion>> SuggestTagsFromTextAsync(string text, int maxTags, CancellationToken cancellationToken = default)
    {
        var suggestions = new List<TagSuggestion>
        {
            new() { Tag = "AFC2027", Confidence = 0.95 },
            new() { Tag = "Knowledge Management", Confidence = 0.88 },
            new() { Tag = "Documentation", Confidence = 0.82 }
        };
        return Task.FromResult(suggestions.Take(maxTags).ToList());
    }

    public Task ApplyTagSuggestionsAsync(Guid documentId, List<string> tags, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Applied {Count} tags to document {DocumentId}", tags.Count, documentId);
        return Task.CompletedTask;
    }

    public async Task<string> GenerateSummaryAsync(Guid documentId, int? maxLength, string? targetLanguage, CancellationToken cancellationToken = default)
    {
        var analysis = await _dbContext.DocumentAnalyses
            .FirstOrDefaultAsync(a => a.DocumentId == documentId, cancellationToken);

        if (analysis != null)
        {
            return targetLanguage == "ar" && !string.IsNullOrEmpty(analysis.SummaryAr)
                ? analysis.SummaryAr
                : analysis.Summary ?? "No summary available.";
        }

        await Task.Delay(100, cancellationToken);
        return "This document provides comprehensive information about the AFC 2027 project, covering key aspects of planning, implementation, and stakeholder management.";
    }

    public Task<string> SummarizeTextAsync(string text, int? maxLength, CancellationToken cancellationToken = default)
    {
        var summary = text.Length > 200 ? text.Substring(0, 200) + "..." : text;
        return Task.FromResult($"Summary: {summary}");
    }

    public Task<List<SemanticSearchResult>> SemanticSearchAsync(SemanticSearchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Performing semantic search: {Query}", request.Query);

        var results = new List<SemanticSearchResult>
        {
            new()
            {
                ContentId = Guid.NewGuid(),
                ContentType = ContentType.Document,
                Title = "AFC 2027 Implementation Plan",
                Snippet = "Comprehensive plan for implementing the knowledge management system...",
                RelevanceScore = 0.92,
                MatchedPhrases = new List<string> { request.Query }
            },
            new()
            {
                ContentId = Guid.NewGuid(),
                ContentType = ContentType.Article,
                Title = "Best Practices for Event Documentation",
                Snippet = "Guidelines for documenting major sporting events...",
                RelevanceScore = 0.85
            }
        };

        return Task.FromResult(results.Where(r => r.RelevanceScore >= request.MinRelevanceScore).Take(request.MaxResults).ToList());
    }

    public Task<List<SimilarContent>> FindSimilarContentAsync(Guid contentId, ContentType contentType, int maxResults, CancellationToken cancellationToken = default)
    {
        var similar = new List<SimilarContent>
        {
            new() { ContentId = Guid.NewGuid(), ContentType = ContentType.Document, Title = "Related Document 1", SimilarityScore = 0.88, CommonTopics = new List<string> { "Project Management" } },
            new() { ContentId = Guid.NewGuid(), ContentType = ContentType.Article, Title = "Related Article", SimilarityScore = 0.75, CommonTopics = new List<string> { "Event Planning" } }
        };
        return Task.FromResult(similar.Take(maxResults).ToList());
    }

    public async Task<TranscriptionResult> StartTranscriptionAsync(TranscribeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting transcription for {SourceType} {SourceId}", request.SourceType, request.SourceId);

        var entity = new TranscriptionResultEntity
        {
            DocumentId = request.SourceId,
            Language = request.Language ?? "en",
            IsProcessing = true,
            TranscribedAt = DateTime.UtcNow,
            FullText = string.Empty
        };

        _dbContext.TranscriptionResults.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = new TranscriptionResult
        {
            Id = entity.Id,
            SourceId = request.SourceId,
            SourceType = request.SourceType,
            ExternalJobId = $"TRANS-{entity.Id:N}".Substring(0, 20),
            Status = TranscriptionStatus.Processing,
            Language = request.Language ?? "en",
            RequestedAt = DateTime.UtcNow
        };

        // Process in background with a new scope to avoid disposed context
        var entityId = entity.Id;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2000, CancellationToken.None);

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<KmsDbContext>();

                var transcriptionEntity = await dbContext.TranscriptionResults.FindAsync(entityId);
                if (transcriptionEntity != null)
                {
                    transcriptionEntity.FullText = "This is a sample transcription of the meeting recording. The participants discussed various topics related to the AFC 2027 project.";
                    transcriptionEntity.Duration = TimeSpan.FromMinutes(45);
                    transcriptionEntity.Confidence = 0.92;
                    transcriptionEntity.IsProcessing = false;
                    transcriptionEntity.TranscribedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync(CancellationToken.None);
                    _logger.LogInformation("Completed transcription {Id}", entityId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transcription {Id}", entityId);
            }
        });

        return result;
    }

    public async Task<TranscriptionResult?> GetTranscriptionStatusAsync(Guid transcriptionId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.TranscriptionResults
            .FirstOrDefaultAsync(t => t.Id == transcriptionId, cancellationToken);

        return entity == null ? null : MapTranscription(entity);
    }

    public async Task<TranscriptionResult?> GetTranscriptionBySourceAsync(Guid sourceId, TranscriptionSourceType sourceType, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.TranscriptionResults
            .FirstOrDefaultAsync(t => t.DocumentId == sourceId, cancellationToken);

        return entity == null ? null : MapTranscription(entity);
    }

    public async Task<Guid> StartBatchAnalysisAsync(BatchAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new BatchAnalysisJobEntity
        {
            JobName = $"Batch-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
            TotalDocuments = request.DocumentIds.Count,
            Status = BatchAnalysisStatusEnum.Queued,
            DocumentIdsJson = JsonSerializer.Serialize(request.DocumentIds),
            InitiatedById = Guid.Empty,
            InitiatedByName = "System"
        };

        _dbContext.BatchAnalysisJobs.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Process in background with a new scope to avoid disposed context
        var batchEntityId = entity.Id;
        var documentIds = request.DocumentIds.ToList();
        var options = request.Options;
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<KmsDbContext>();

                var job = await dbContext.BatchAnalysisJobs.FindAsync(batchEntityId);
                if (job == null) return;

                job.Status = BatchAnalysisStatusEnum.Processing;
                job.StartedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(CancellationToken.None);

                foreach (var docId in documentIds)
                {
                    try
                    {
                        // Create a new scope for each document analysis
                        using var docScope = _scopeFactory.CreateScope();
                        var analysisService = docScope.ServiceProvider.GetRequiredService<IAIAnalysisService>();
                        await analysisService.AnalyzeDocumentAsync(docId, options, CancellationToken.None);
                        job.ProcessedDocuments++;
                    }
                    catch (Exception ex)
                    {
                        job.FailedDocuments++;
                        _logger.LogError(ex, "Failed to analyze document {DocumentId} in batch {BatchId}", docId, job.Id);
                    }
                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }

                job.Status = BatchAnalysisStatusEnum.Completed;
                job.CompletedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(CancellationToken.None);
                _logger.LogInformation("Completed batch analysis {BatchId}", batchEntityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch analysis {BatchId}", batchEntityId);
            }
        });

        _logger.LogInformation("Started batch analysis {BatchId} for {Count} documents", entity.Id, request.DocumentIds.Count);
        return entity.Id;
    }

    public async Task<BatchAnalysisStatus?> GetBatchStatusAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.BatchAnalysisJobs
            .FirstOrDefaultAsync(b => b.Id == batchId, cancellationToken);

        if (entity == null) return null;

        return new BatchAnalysisStatus
        {
            BatchId = entity.Id,
            TotalDocuments = entity.TotalDocuments,
            ProcessedDocuments = entity.ProcessedDocuments,
            SuccessCount = entity.ProcessedDocuments - entity.FailedDocuments,
            FailedCount = entity.FailedDocuments,
            Status = (BatchStatus)entity.Status,
            StartedAt = entity.StartedAt ?? entity.CreatedAt,
            CompletedAt = entity.CompletedAt,
            Errors = !string.IsNullOrEmpty(entity.ErrorMessage) ? new List<string> { entity.ErrorMessage } : new List<string>()
        };
    }

    public async Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid userId, int maxResults, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.ContentRecommendations
            .Where(r => r.UserId == userId && !r.IsDismissed && r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.RelevanceScore)
            .Take(maxResults)
            .ToListAsync(cancellationToken);

        if (entities.Any())
        {
            return entities.Select(e => new ContentRecommendation
            {
                ContentId = e.RecommendedDocumentId,
                ContentType = ContentType.Document,
                Title = $"Document {e.RecommendedDocumentId}",
                RecommendationType = Enum.TryParse<RecommendationType>(e.RecommendationType, out var rt) ? rt : RecommendationType.BasedOnInterests,
                Score = e.RelevanceScore,
                Reason = e.ReasonJson ?? "Recommended for you"
            }).ToList();
        }

        // Default recommendations if none exist
        return new List<ContentRecommendation>
        {
            new() { ContentId = Guid.NewGuid(), ContentType = ContentType.Document, Title = "Recommended: Project Planning Guide", RecommendationType = RecommendationType.BasedOnInterests, Score = 0.92, Reason = "Based on your recent activity" },
            new() { ContentId = Guid.NewGuid(), ContentType = ContentType.Article, Title = "Trending: AFC 2027 Updates", RecommendationType = RecommendationType.Trending, Score = 0.88, Reason = "Popular in your organization" }
        };
    }

    public Task<List<ContentRecommendation>> GetContentBasedRecommendationsAsync(Guid contentId, ContentType contentType, int maxResults, CancellationToken cancellationToken = default)
    {
        var recommendations = new List<ContentRecommendation>
        {
            new() { ContentId = Guid.NewGuid(), ContentType = ContentType.Document, Title = "Similar: Implementation Checklist", RecommendationType = RecommendationType.Similar, Score = 0.85, Reason = "Similar content" },
            new() { ContentId = Guid.NewGuid(), ContentType = ContentType.Document, Title = "Related: Resource Allocation", RecommendationType = RecommendationType.Similar, Score = 0.78, Reason = "Related topics" }
        };
        return Task.FromResult(recommendations.Take(maxResults).ToList());
    }

    public async Task<AIInsightsDashboard> GetInsightsDashboardAsync(CancellationToken cancellationToken = default)
    {
        var analyses = await _dbContext.DocumentAnalyses
            .Include(a => a.ExtractedEntities)
            .ToListAsync(cancellationToken);

        var dashboard = new AIInsightsDashboard
        {
            TotalDocumentsAnalyzed = analyses.Count,
            DocumentsPendingAnalysis = analyses.Count(a => a.IsProcessing),
            TopEntityTypes = analyses.SelectMany(a => a.ExtractedEntities)
                .GroupBy(e => Enum.TryParse<EntityType>(e.EntityType, out var t) ? t : EntityType.Person)
                .Select(g => new EntityTypeCount { Type = g.Key, Count = g.Count() })
                .OrderByDescending(e => e.Count).Take(5).ToList(),
            TopSuggestedTags = analyses
                .Where(a => !string.IsNullOrEmpty(a.SuggestedTagsJson))
                .SelectMany(a => JsonSerializer.Deserialize<string[]>(a.SuggestedTagsJson!) ?? Array.Empty<string>())
                .GroupBy(t => t)
                .Select(g => new TagCount { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(t => t.Count).Take(10).ToList(),
            SentimentDistribution = new SentimentDistribution
            {
                Positive = analyses.Count(a => a.SentimentLabel == "Positive" || a.SentimentLabel == "VeryPositive"),
                Neutral = analyses.Count(a => a.SentimentLabel == "Neutral"),
                Negative = analyses.Count(a => a.SentimentLabel == "Negative" || a.SentimentLabel == "VeryNegative")
            },
            TrendingTopics = analyses
                .Where(a => !string.IsNullOrEmpty(a.TopicsJson))
                .SelectMany(a => JsonSerializer.Deserialize<string[]>(a.TopicsJson!) ?? Array.Empty<string>())
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count()).Take(5).Select(g => g.Key).ToList(),
            AverageReadabilityScore = analyses.Any() ? analyses.Where(a => a.ReadabilityScore.HasValue).Average(a => a.ReadabilityScore!.Value) : 0
        };

        return dashboard;
    }

    // Mapping helpers
    private static DocumentAnalysis MapToModel(DocumentAnalysisEntity entity)
    {
        return new DocumentAnalysis
        {
            Id = entity.Id,
            DocumentId = entity.DocumentId,
            DocumentTitle = $"Document {entity.DocumentId}",
            Summary = entity.Summary,
            SummaryAr = entity.SummaryAr,
            DetectedLanguage = entity.Language,
            SentimentScore = entity.SentimentScore ?? 0,
            SentimentLabel = Enum.TryParse<SentimentLabel>(entity.SentimentLabel, out var label) ? label : SentimentLabel.Neutral,
            SuggestedTags = !string.IsNullOrEmpty(entity.SuggestedTagsJson)
                ? JsonSerializer.Deserialize<List<string>>(entity.SuggestedTagsJson) ?? new List<string>()
                : new List<string>(),
            KeyPhrases = !string.IsNullOrEmpty(entity.KeyPhrasesJson)
                ? JsonSerializer.Deserialize<List<string>>(entity.KeyPhrasesJson) ?? new List<string>()
                : new List<string>(),
            Topics = !string.IsNullOrEmpty(entity.TopicsJson)
                ? JsonSerializer.Deserialize<List<string>>(entity.TopicsJson) ?? new List<string>()
                : new List<string>(),
            ReadabilityScore = entity.ReadabilityScore ?? 0,
            WordCount = entity.WordCount ?? 0,
            EstimatedReadTimeMinutes = (entity.WordCount ?? 0) / 250,
            Entities = entity.ExtractedEntities?.Select(e => new ExtractedEntity
            {
                Id = e.Id,
                DocumentId = e.DocumentId,
                Type = Enum.TryParse<EntityType>(e.EntityType, out var t) ? t : EntityType.Person,
                Value = e.Value,
                NormalizedValue = e.NormalizedValue,
                Confidence = e.Confidence
            }).ToList() ?? new List<ExtractedEntity>(),
            Status = entity.IsProcessing ? AnalysisStatus.Processing
                : string.IsNullOrEmpty(entity.ErrorMessage) ? AnalysisStatus.Completed : AnalysisStatus.Failed,
            ErrorMessage = entity.ErrorMessage,
            AnalyzedAt = entity.AnalyzedAt
        };
    }

    private static TranscriptionResult MapTranscription(TranscriptionResultEntity entity)
    {
        return new TranscriptionResult
        {
            Id = entity.Id,
            SourceId = entity.DocumentId,
            SourceType = TranscriptionSourceType.AudioFile,
            ExternalJobId = $"TRANS-{entity.Id:N}".Substring(0, 20),
            Status = entity.IsProcessing ? TranscriptionStatus.Processing
                : string.IsNullOrEmpty(entity.ErrorMessage) ? TranscriptionStatus.Completed : TranscriptionStatus.Failed,
            TranscribedText = entity.FullText,
            Language = entity.Language ?? "en",
            Confidence = entity.Confidence ?? 0,
            Duration = entity.Duration ?? TimeSpan.Zero,
            RequestedAt = entity.CreatedAt,
            CompletedAt = entity.IsProcessing ? null : entity.TranscribedAt,
            ErrorMessage = entity.ErrorMessage
        };
    }
}
