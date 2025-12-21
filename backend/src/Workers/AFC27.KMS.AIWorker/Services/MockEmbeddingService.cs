using Microsoft.Extensions.Logging;

namespace AFC27.KMS.AIWorker.Services;

/// <summary>
/// Mock embedding service for development.
/// Generates deterministic pseudo-embeddings based on text content.
/// Replace with real Intalio AI client when credentials are available.
/// </summary>
public class MockEmbeddingService
{
    private readonly ILogger<MockEmbeddingService> _logger;
    private const int EmbeddingDimension = 1536; // Match common embedding dimensions

    public MockEmbeddingService(ILogger<MockEmbeddingService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates a mock embedding vector for the given text.
    /// </summary>
    public Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Generating mock embedding for text of length {Length}", text.Length);

        // Generate deterministic pseudo-random embedding based on text hash
        var embedding = GeneratePseudoEmbedding(text);

        return Task.FromResult(embedding);
    }

    /// <summary>
    /// Generates mock embeddings for multiple texts in batch.
    /// </summary>
    public Task<IEnumerable<float[]>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts,
        CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        _logger.LogDebug("Generating mock embeddings for {Count} texts", textList.Count);

        var embeddings = textList.Select(GeneratePseudoEmbedding);
        return Task.FromResult(embeddings);
    }

    /// <summary>
    /// Calculates cosine similarity between two embeddings.
    /// </summary>
    public float CalculateSimilarity(float[] embedding1, float[] embedding2)
    {
        if (embedding1.Length != embedding2.Length)
        {
            throw new ArgumentException("Embeddings must have the same dimension");
        }

        var dotProduct = 0f;
        var magnitude1 = 0f;
        var magnitude2 = 0f;

        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            magnitude1 += embedding1[i] * embedding1[i];
            magnitude2 += embedding2[i] * embedding2[i];
        }

        magnitude1 = MathF.Sqrt(magnitude1);
        magnitude2 = MathF.Sqrt(magnitude2);

        if (magnitude1 == 0 || magnitude2 == 0)
            return 0;

        return dotProduct / (magnitude1 * magnitude2);
    }

    /// <summary>
    /// Finds the most similar embeddings from a collection.
    /// </summary>
    public IEnumerable<(int Index, float Similarity)> FindMostSimilar(
        float[] queryEmbedding,
        IEnumerable<float[]> embeddings,
        int topK = 5)
    {
        return embeddings
            .Select((embedding, index) => (Index: index, Similarity: CalculateSimilarity(queryEmbedding, embedding)))
            .OrderByDescending(x => x.Similarity)
            .Take(topK);
    }

    private float[] GeneratePseudoEmbedding(string text)
    {
        var embedding = new float[EmbeddingDimension];

        // Use text hash as seed for deterministic results
        var hash = text.GetHashCode();
        var random = new Random(hash);

        // Generate normalized random values
        var sum = 0f;
        for (int i = 0; i < EmbeddingDimension; i++)
        {
            embedding[i] = (float)(random.NextDouble() * 2 - 1);
            sum += embedding[i] * embedding[i];
        }

        // Normalize to unit vector
        var magnitude = MathF.Sqrt(sum);
        for (int i = 0; i < EmbeddingDimension; i++)
        {
            embedding[i] /= magnitude;
        }

        // Add some text-based features to make similar texts have similar embeddings
        AddTextFeatures(embedding, text);

        // Re-normalize after adding features
        sum = 0f;
        for (int i = 0; i < EmbeddingDimension; i++)
        {
            sum += embedding[i] * embedding[i];
        }
        magnitude = MathF.Sqrt(sum);
        for (int i = 0; i < EmbeddingDimension; i++)
        {
            embedding[i] /= magnitude;
        }

        return embedding;
    }

    private static void AddTextFeatures(float[] embedding, string text)
    {
        // Add simple text features to make embeddings somewhat meaningful
        var lowerText = text.ToLowerInvariant();

        // Length feature
        embedding[0] += Math.Min(text.Length / 1000f, 1f) * 0.1f;

        // Word count feature
        var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        embedding[1] += Math.Min(wordCount / 100f, 1f) * 0.1f;

        // Common word features
        var commonWords = new[] { "the", "and", "is", "in", "to", "of", "a", "for", "on", "with" };
        for (int i = 0; i < commonWords.Length; i++)
        {
            var count = CountOccurrences(lowerText, commonWords[i]);
            embedding[10 + i] += Math.Min(count / 10f, 1f) * 0.05f;
        }

        // Language hint (Arabic vs English)
        var arabicChars = text.Count(c => c >= 0x0600 && c <= 0x06FF);
        embedding[30] += (arabicChars > text.Length / 4) ? 0.5f : -0.5f;

        // Question feature
        if (text.Contains('?'))
        {
            embedding[40] += 0.3f;
        }

        // Sentence count feature
        var sentenceCount = text.Count(c => c == '.' || c == '!' || c == '?');
        embedding[50] += Math.Min(sentenceCount / 20f, 1f) * 0.1f;
    }

    private static int CountOccurrences(string text, string word)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(word, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += word.Length;
        }
        return count;
    }
}
