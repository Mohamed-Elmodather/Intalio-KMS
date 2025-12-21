using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for block-based content editing operations.
/// </summary>
public class BlockEditorService : IBlockEditorService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<BlockEditorService> _logger;

    public BlockEditorService(
        DbContext dbContext,
        ILogger<BlockEditorService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ContentBlockDto>> GetBlocksAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var blocks = await _dbContext.Set<ContentBlock>()
            .AsNoTracking()
            .Where(b => b.ArticleId == articleId && !b.IsDeleted)
            .OrderBy(b => b.Order)
            .ToListAsync(cancellationToken);

        // Build hierarchical structure
        var blockDtos = blocks.Select(MapToDto).ToList();
        return BuildHierarchy(blockDtos);
    }

    public async Task<ContentBlockDto> CreateBlockAsync(
        Guid articleId,
        CreateBlockRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Determine order
        int order;
        if (request.Order.HasValue)
        {
            order = request.Order.Value;
        }
        else if (request.InsertAfterId.HasValue)
        {
            var afterBlock = await _dbContext.Set<ContentBlock>()
                .FirstOrDefaultAsync(b => b.Id == request.InsertAfterId.Value, cancellationToken);

            if (afterBlock != null)
            {
                order = afterBlock.Order + 1;
                // Shift subsequent blocks
                await ShiftBlocksAsync(articleId, order, 1, request.ParentBlockId, cancellationToken);
            }
            else
            {
                order = await GetNextOrderAsync(articleId, request.ParentBlockId, cancellationToken);
            }
        }
        else
        {
            order = await GetNextOrderAsync(articleId, request.ParentBlockId, cancellationToken);
        }

        var blockType = Enum.TryParse<BlockType>(request.Type, out var type) ? type : BlockType.Paragraph;

        var block = ContentBlock.Create(
            articleId,
            blockType,
            request.Content,
            order,
            request.ContentArabic,
            request.Metadata,
            request.ParentBlockId);

        _dbContext.Set<ContentBlock>().Add(block);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created block {BlockId} of type {BlockType} in article {ArticleId} by user {UserId}",
            block.Id, blockType, articleId, userId);

        return MapToDto(block);
    }

    public async Task<bool> UpdateBlockAsync(
        Guid blockId,
        UpdateBlockRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var block = await _dbContext.Set<ContentBlock>()
            .FirstOrDefaultAsync(b => b.Id == blockId && !b.IsDeleted, cancellationToken);

        if (block == null)
        {
            _logger.LogWarning("Block {BlockId} not found for update", blockId);
            return false;
        }

        if (request.Content != null || request.ContentArabic != null)
        {
            block.UpdateContent(
                request.Content ?? block.Content,
                request.ContentArabic ?? block.ContentArabic);
        }

        if (request.Metadata != null)
        {
            block.UpdateMetadata(request.Metadata);
        }

        if (request.Type != null && Enum.TryParse<BlockType>(request.Type, out var newType))
        {
            block.ChangeType(newType);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated block {BlockId} by user {UserId}",
            blockId, userId);

        return true;
    }

    public async Task<bool> DeleteBlockAsync(
        Guid blockId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var block = await _dbContext.Set<ContentBlock>()
            .Include(b => b.ChildBlocks)
            .FirstOrDefaultAsync(b => b.Id == blockId && !b.IsDeleted, cancellationToken);

        if (block == null)
        {
            _logger.LogWarning("Block {BlockId} not found for deletion", blockId);
            return false;
        }

        // Recursively delete children
        await DeleteBlockRecursiveAsync(block, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Deleted block {BlockId} and its children by user {UserId}",
            blockId, userId);

        return true;
    }

    public async Task<bool> ReorderBlocksAsync(
        Guid articleId,
        IReadOnlyList<Guid> blockIds,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var blocks = await _dbContext.Set<ContentBlock>()
            .Where(b => b.ArticleId == articleId && !b.IsDeleted && blockIds.Contains(b.Id))
            .ToListAsync(cancellationToken);

        if (blocks.Count != blockIds.Count)
        {
            _logger.LogWarning(
                "Reorder mismatch: expected {Expected} blocks, found {Found}",
                blockIds.Count, blocks.Count);
            return false;
        }

        for (int i = 0; i < blockIds.Count; i++)
        {
            var block = blocks.First(b => b.Id == blockIds[i]);
            block.UpdateOrder(i);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Reordered {BlockCount} blocks in article {ArticleId} by user {UserId}",
            blockIds.Count, articleId, userId);

        return true;
    }

    public async Task<bool> MoveBlockAsync(
        Guid blockId,
        int newPosition,
        Guid? newParentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var block = await _dbContext.Set<ContentBlock>()
            .FirstOrDefaultAsync(b => b.Id == blockId && !b.IsDeleted, cancellationToken);

        if (block == null)
        {
            _logger.LogWarning("Block {BlockId} not found for move", blockId);
            return false;
        }

        var oldParentId = block.ParentBlockId;
        var oldOrder = block.Order;

        // Remove from old position
        await ShiftBlocksAsync(block.ArticleId, oldOrder + 1, -1, oldParentId, cancellationToken);

        // Insert at new position
        await ShiftBlocksAsync(block.ArticleId, newPosition, 1, newParentId, cancellationToken);

        block.SetParent(newParentId);
        block.UpdateOrder(newPosition);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Moved block {BlockId} to position {Position} with parent {ParentId} by user {UserId}",
            blockId, newPosition, newParentId, userId);

        return true;
    }

    public async Task<ContentBlockDto?> DuplicateBlockAsync(
        Guid blockId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var original = await _dbContext.Set<ContentBlock>()
            .Include(b => b.ChildBlocks.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(b => b.Id == blockId && !b.IsDeleted, cancellationToken);

        if (original == null)
        {
            _logger.LogWarning("Block {BlockId} not found for duplication", blockId);
            return null;
        }

        // Shift blocks to make room
        await ShiftBlocksAsync(original.ArticleId, original.Order + 1, 1, original.ParentBlockId, cancellationToken);

        // Create duplicate
        var duplicate = ContentBlock.Create(
            original.ArticleId,
            original.Type,
            original.Content,
            original.Order + 1,
            original.ContentArabic,
            original.Metadata,
            original.ParentBlockId);

        _dbContext.Set<ContentBlock>().Add(duplicate);

        // Duplicate children
        await DuplicateChildrenAsync(original, duplicate.Id, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Duplicated block {OriginalBlockId} as {DuplicateBlockId} by user {UserId}",
            blockId, duplicate.Id, userId);

        return MapToDto(duplicate);
    }

    public async Task<string> RenderToHtmlAsync(
        Guid articleId,
        string language = "en",
        CancellationToken cancellationToken = default)
    {
        var blocks = await GetBlocksAsync(articleId, cancellationToken);
        var html = new StringBuilder();

        foreach (var block in blocks)
        {
            html.Append(RenderBlockToHtml(block, language));
        }

        return html.ToString();
    }

    public async Task<IReadOnlyList<ContentBlockDto>> ImportFromHtmlAsync(
        Guid articleId,
        string html,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Simple HTML parser - in production, use a proper HTML parsing library
        var blocks = new List<ContentBlockDto>();
        var order = await GetNextOrderAsync(articleId, null, cancellationToken);

        // Basic parsing: split by block-level elements
        var lines = html.Split(new[] { "<p>", "</p>", "<div>", "</div>" },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            var content = StripHtmlTags(line);
            if (string.IsNullOrWhiteSpace(content))
                continue;

            var request = new CreateBlockRequest
            {
                Type = "Paragraph",
                Content = content,
                Order = order++
            };

            var block = await CreateBlockAsync(articleId, request, userId, cancellationToken);
            blocks.Add(block);
        }

        _logger.LogInformation(
            "Imported {BlockCount} blocks from HTML into article {ArticleId} by user {UserId}",
            blocks.Count, articleId, userId);

        return blocks;
    }

    private async Task<int> GetNextOrderAsync(
        Guid articleId,
        Guid? parentBlockId,
        CancellationToken cancellationToken)
    {
        var maxOrder = await _dbContext.Set<ContentBlock>()
            .Where(b => b.ArticleId == articleId &&
                        b.ParentBlockId == parentBlockId &&
                        !b.IsDeleted)
            .MaxAsync(b => (int?)b.Order, cancellationToken);

        return (maxOrder ?? -1) + 1;
    }

    private async Task ShiftBlocksAsync(
        Guid articleId,
        int fromOrder,
        int shift,
        Guid? parentBlockId,
        CancellationToken cancellationToken)
    {
        var blocksToShift = await _dbContext.Set<ContentBlock>()
            .Where(b => b.ArticleId == articleId &&
                        b.ParentBlockId == parentBlockId &&
                        b.Order >= fromOrder &&
                        !b.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var block in blocksToShift)
        {
            block.UpdateOrder(block.Order + shift);
        }
    }

    private async Task DeleteBlockRecursiveAsync(
        ContentBlock block,
        CancellationToken cancellationToken)
    {
        // Load children if not already loaded
        if (!_dbContext.Entry(block).Collection(b => b.ChildBlocks).IsLoaded)
        {
            await _dbContext.Entry(block).Collection(b => b.ChildBlocks).LoadAsync(cancellationToken);
        }

        foreach (var child in block.ChildBlocks.Where(c => !c.IsDeleted))
        {
            await DeleteBlockRecursiveAsync(child, cancellationToken);
        }

        block.Delete();
    }

    private async Task DuplicateChildrenAsync(
        ContentBlock original,
        Guid newParentId,
        CancellationToken cancellationToken)
    {
        foreach (var child in original.ChildBlocks.Where(c => !c.IsDeleted))
        {
            var childDuplicate = ContentBlock.Create(
                original.ArticleId,
                child.Type,
                child.Content,
                child.Order,
                child.ContentArabic,
                child.Metadata,
                newParentId);

            _dbContext.Set<ContentBlock>().Add(childDuplicate);

            // Recursively duplicate grandchildren
            if (child.ChildBlocks.Any(c => !c.IsDeleted))
            {
                await _dbContext.Entry(child).Collection(c => c.ChildBlocks).LoadAsync(cancellationToken);
                await DuplicateChildrenAsync(child, childDuplicate.Id, cancellationToken);
            }
        }
    }

    private string RenderBlockToHtml(ContentBlockDto block, string language)
    {
        var content = language == "ar" && !string.IsNullOrEmpty(block.ContentArabic)
            ? block.ContentArabic
            : block.Content;

        var html = block.Type switch
        {
            "Heading" => RenderHeading(content, block.Metadata),
            "BulletList" => RenderList(block, language, false),
            "NumberedList" => RenderList(block, language, true),
            "Quote" => $"<blockquote>{content}</blockquote>\n",
            "Code" => RenderCode(content, block.Metadata),
            "Image" => RenderImage(block.Metadata),
            "Video" => RenderVideo(block.Metadata),
            "Divider" => "<hr />\n",
            "Callout" => RenderCallout(content, block.Metadata),
            _ => $"<p>{content}</p>\n"
        };

        // Render children
        if (block.Children.Any())
        {
            foreach (var child in block.Children)
            {
                html += RenderBlockToHtml(child, language);
            }
        }

        return html;
    }

    private string RenderHeading(string content, string? metadata)
    {
        var level = 1;
        if (!string.IsNullOrEmpty(metadata))
        {
            try
            {
                var meta = JsonSerializer.Deserialize<HeadingMetadata>(metadata);
                level = meta?.Level ?? 1;
            }
            catch { }
        }
        level = Math.Clamp(level, 1, 6);
        return $"<h{level}>{content}</h{level}>\n";
    }

    private string RenderList(ContentBlockDto block, string language, bool ordered)
    {
        var tag = ordered ? "ol" : "ul";
        var html = new StringBuilder($"<{tag}>\n");

        foreach (var child in block.Children)
        {
            var content = language == "ar" && !string.IsNullOrEmpty(child.ContentArabic)
                ? child.ContentArabic
                : child.Content;
            html.Append($"  <li>{content}</li>\n");
        }

        html.Append($"</{tag}>\n");
        return html.ToString();
    }

    private string RenderCode(string content, string? metadata)
    {
        var language = "";
        if (!string.IsNullOrEmpty(metadata))
        {
            try
            {
                var meta = JsonSerializer.Deserialize<CodeMetadata>(metadata);
                language = meta?.Language ?? "";
            }
            catch { }
        }
        return $"<pre><code class=\"language-{language}\">{System.Web.HttpUtility.HtmlEncode(content)}</code></pre>\n";
    }

    private string RenderImage(string? metadata)
    {
        if (string.IsNullOrEmpty(metadata))
            return "";

        try
        {
            var meta = JsonSerializer.Deserialize<ImageMetadata>(metadata);
            if (meta == null)
                return "";

            var altText = System.Web.HttpUtility.HtmlEncode(meta.AltText ?? "");
            var html = $"<figure><img src=\"{meta.Url}\" alt=\"{altText}\"";
            if (meta.Width.HasValue)
                html += $" width=\"{meta.Width}\"";
            if (meta.Height.HasValue)
                html += $" height=\"{meta.Height}\"";
            html += " />";
            if (!string.IsNullOrEmpty(meta.Caption))
                html += $"<figcaption>{meta.Caption}</figcaption>";
            html += "</figure>\n";
            return html;
        }
        catch
        {
            return "";
        }
    }

    private string RenderVideo(string? metadata)
    {
        if (string.IsNullOrEmpty(metadata))
            return "";

        try
        {
            var meta = JsonSerializer.Deserialize<VideoMetadata>(metadata);
            if (meta == null)
                return "";

            return $"<video src=\"{meta.Url}\" controls></video>\n";
        }
        catch
        {
            return "";
        }
    }

    private string RenderCallout(string content, string? metadata)
    {
        var type = "info";
        if (!string.IsNullOrEmpty(metadata))
        {
            try
            {
                var meta = JsonSerializer.Deserialize<CalloutMetadata>(metadata);
                type = meta?.Type ?? "info";
            }
            catch { }
        }
        return $"<div class=\"callout callout-{type}\">{content}</div>\n";
    }

    private static string StripHtmlTags(string html)
    {
        return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "");
    }

    private static ContentBlockDto MapToDto(ContentBlock block)
    {
        return new ContentBlockDto
        {
            Id = block.Id,
            ArticleId = block.ArticleId,
            Type = block.Type.ToString(),
            Content = block.Content,
            ContentArabic = block.ContentArabic,
            Order = block.Order,
            Metadata = block.Metadata,
            ParentBlockId = block.ParentBlockId,
            Children = Array.Empty<ContentBlockDto>(),
            CreatedAt = block.CreatedAt,
            ModifiedAt = block.ModifiedAt
        };
    }

    private static IReadOnlyList<ContentBlockDto> BuildHierarchy(List<ContentBlockDto> flatBlocks)
    {
        var lookup = flatBlocks.ToDictionary(b => b.Id);
        var roots = new List<ContentBlockDto>();

        foreach (var block in flatBlocks)
        {
            if (block.ParentBlockId == null)
            {
                roots.Add(block);
            }
            else if (lookup.TryGetValue(block.ParentBlockId.Value, out var parent))
            {
                var children = parent.Children.ToList();
                children.Add(block);
                lookup[parent.Id] = parent with { Children = children };
            }
        }

        // Re-fetch roots with updated children
        return flatBlocks
            .Where(b => b.ParentBlockId == null)
            .Select(b => lookup[b.Id])
            .OrderBy(b => b.Order)
            .ToList();
    }

    // Metadata classes for JSON deserialization
    private record HeadingMetadata(int Level);
    private record CodeMetadata(string Language, bool ShowLineNumbers = true);
    private record ImageMetadata(string Url, string? AltText, string? Caption, int? Width, int? Height);
    private record VideoMetadata(string Url, string? ThumbnailUrl, string? Caption);
    private record CalloutMetadata(string Type, string? Icon);
}
