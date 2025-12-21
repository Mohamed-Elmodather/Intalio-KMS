using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AFC27.KMS.AIWorker.Services;

/// <summary>
/// Service for splitting text content into chunks suitable for embedding generation.
/// </summary>
public class ChunkingService
{
    private readonly ChunkingOptions _options;
    private readonly ILogger<ChunkingService> _logger;

    public ChunkingService(
        IOptions<ChunkingOptions> options,
        ILogger<ChunkingService> logger)
    {
        _options = options?.Value ?? new ChunkingOptions();
        _logger = logger;
    }

    /// <summary>
    /// Splits text into overlapping chunks for embedding generation.
    /// </summary>
    public IEnumerable<TextChunk> ChunkText(string text, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        _logger.LogDebug("Chunking text of {Length} characters", text.Length);

        var chunks = new List<TextChunk>();
        var sentences = SplitIntoSentences(text);
        var currentChunk = new System.Text.StringBuilder();
        var chunkIndex = 0;
        var overlapBuffer = new Queue<string>();

        foreach (var sentence in sentences)
        {
            // Check if adding this sentence would exceed chunk size
            if (currentChunk.Length + sentence.Length > _options.ChunkSize && currentChunk.Length > 0)
            {
                // Yield current chunk
                yield return CreateChunk(currentChunk.ToString(), chunkIndex++, title);

                // Start new chunk with overlap from previous
                currentChunk.Clear();
                foreach (var overlapSentence in overlapBuffer)
                {
                    currentChunk.Append(overlapSentence);
                }
                overlapBuffer.Clear();
            }

            currentChunk.Append(sentence);

            // Maintain overlap buffer
            overlapBuffer.Enqueue(sentence);
            while (GetTotalLength(overlapBuffer) > _options.ChunkOverlap && overlapBuffer.Count > 1)
            {
                overlapBuffer.Dequeue();
            }
        }

        // Don't forget the last chunk
        if (currentChunk.Length > 0)
        {
            yield return CreateChunk(currentChunk.ToString(), chunkIndex, title);
        }

        _logger.LogDebug("Created {Count} chunks from text", chunkIndex + 1);
    }

    /// <summary>
    /// Chunks text with metadata preservation.
    /// </summary>
    public IEnumerable<TextChunk> ChunkTextWithMetadata(
        string text,
        string? title,
        Dictionary<string, string>? metadata)
    {
        foreach (var chunk in ChunkText(text, title))
        {
            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    chunk.Metadata[kvp.Key] = kvp.Value;
                }
            }
            yield return chunk;
        }
    }

    /// <summary>
    /// Splits text by paragraphs first, then sentences if needed.
    /// Better for preserving context.
    /// </summary>
    public IEnumerable<TextChunk> ChunkByParagraph(string text, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        var paragraphs = text.Split(
            new[] { "\n\n", "\r\n\r\n" },
            StringSplitOptions.RemoveEmptyEntries);

        var currentChunk = new System.Text.StringBuilder();
        var chunkIndex = 0;

        foreach (var paragraph in paragraphs)
        {
            var trimmedParagraph = paragraph.Trim();

            if (string.IsNullOrWhiteSpace(trimmedParagraph))
                continue;

            // If paragraph alone exceeds chunk size, split it
            if (trimmedParagraph.Length > _options.ChunkSize)
            {
                // Yield current chunk if any
                if (currentChunk.Length > 0)
                {
                    yield return CreateChunk(currentChunk.ToString(), chunkIndex++, title);
                    currentChunk.Clear();
                }

                // Split large paragraph into sentence-based chunks
                foreach (var subChunk in ChunkText(trimmedParagraph, title))
                {
                    subChunk.Index = chunkIndex++;
                    yield return subChunk;
                }
                continue;
            }

            // Check if adding this paragraph would exceed chunk size
            if (currentChunk.Length + trimmedParagraph.Length + 2 > _options.ChunkSize && currentChunk.Length > 0)
            {
                yield return CreateChunk(currentChunk.ToString(), chunkIndex++, title);
                currentChunk.Clear();
            }

            if (currentChunk.Length > 0)
            {
                currentChunk.AppendLine();
                currentChunk.AppendLine();
            }
            currentChunk.Append(trimmedParagraph);
        }

        // Yield final chunk
        if (currentChunk.Length > 0)
        {
            yield return CreateChunk(currentChunk.ToString(), chunkIndex, title);
        }
    }

    private static IEnumerable<string> SplitIntoSentences(string text)
    {
        // Simple sentence splitting - in production use NLP library
        var sentenceEndings = new[] { ". ", "! ", "? ", ".\n", "!\n", "?\n" };
        var currentSentence = new System.Text.StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            currentSentence.Append(text[i]);

            // Check if we're at a sentence ending
            var remaining = text.Substring(i);
            foreach (var ending in sentenceEndings)
            {
                if (remaining.StartsWith(ending[0].ToString()) &&
                    (i == text.Length - 1 || remaining.Length >= ending.Length && remaining.StartsWith(ending)))
                {
                    // Include the punctuation and space
                    if (ending.Length > 1 && i + 1 < text.Length)
                    {
                        currentSentence.Append(text[++i]);
                    }

                    yield return currentSentence.ToString();
                    currentSentence.Clear();
                    break;
                }
            }
        }

        // Don't forget remaining text
        if (currentSentence.Length > 0)
        {
            yield return currentSentence.ToString();
        }
    }

    private TextChunk CreateChunk(string text, int index, string? title)
    {
        return new TextChunk
        {
            Index = index,
            Text = text.Trim(),
            Title = title,
            CharacterCount = text.Length,
            WordCount = CountWords(text)
        };
    }

    private static int GetTotalLength(IEnumerable<string> strings)
    {
        return strings.Sum(s => s.Length);
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { ' ', '\t', '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

/// <summary>
/// Configuration options for text chunking.
/// </summary>
public class ChunkingOptions
{
    public const string SectionName = "AI:Chunking";

    /// <summary>
    /// Target size for each chunk in characters.
    /// </summary>
    public int ChunkSize { get; set; } = 500;

    /// <summary>
    /// Number of characters to overlap between chunks.
    /// </summary>
    public int ChunkOverlap { get; set; } = 50;

    /// <summary>
    /// Minimum chunk size (don't create tiny chunks).
    /// </summary>
    public int MinChunkSize { get; set; } = 100;
}

/// <summary>
/// Represents a chunk of text for embedding.
/// </summary>
public class TextChunk
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Title { get; set; }
    public int CharacterCount { get; set; }
    public int WordCount { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
