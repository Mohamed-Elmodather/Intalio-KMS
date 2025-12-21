namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Collaboration session DTO.
/// </summary>
public record CollaborationSessionDto
{
    public Guid Id { get; init; }
    public Guid ArticleId { get; init; }
    public string SessionId { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastActivityAt { get; init; }
    public IReadOnlyList<ParticipantDto> Participants { get; init; } = Array.Empty<ParticipantDto>();
}

/// <summary>
/// Participant DTO.
/// </summary>
public record ParticipantDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string Color { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime JoinedAt { get; init; }
    public DateTime LastSeenAt { get; init; }
    public CursorPositionDto? Cursor { get; init; }
}

/// <summary>
/// Cursor position DTO.
/// </summary>
public record CursorPositionDto
{
    public Guid? BlockId { get; init; }
    public int? Position { get; init; }
}

/// <summary>
/// Content block DTO.
/// </summary>
public record ContentBlockDto
{
    public Guid Id { get; init; }
    public Guid ArticleId { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public int Order { get; init; }
    public string? Metadata { get; init; }
    public Guid? ParentBlockId { get; init; }
    public IReadOnlyList<ContentBlockDto> Children { get; init; } = Array.Empty<ContentBlockDto>();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Request to create a new block.
/// </summary>
public record CreateBlockRequest
{
    public string Type { get; init; } = "Paragraph";
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public int? Order { get; init; }
    public string? Metadata { get; init; }
    public Guid? ParentBlockId { get; init; }
    public Guid? InsertAfterId { get; init; } // Insert after this block
}

/// <summary>
/// Request to update a block.
/// </summary>
public record UpdateBlockRequest
{
    public string? Content { get; init; }
    public string? ContentArabic { get; init; }
    public string? Metadata { get; init; }
    public string? Type { get; init; }
}

/// <summary>
/// Request to reorder blocks.
/// </summary>
public record ReorderBlocksRequest
{
    public IReadOnlyList<Guid> BlockIds { get; init; } = Array.Empty<Guid>();
}

/// <summary>
/// Request to move a block.
/// </summary>
public record MoveBlockRequest
{
    public int NewPosition { get; init; }
    public Guid? NewParentId { get; init; }
}

/// <summary>
/// Block content for rich text editor.
/// </summary>
public record BlockContentDto
{
    public string Type { get; init; } = string.Empty;
    public string? Text { get; init; }
    public IReadOnlyList<InlineMarkDto>? Marks { get; init; }
    public IReadOnlyList<BlockContentDto>? Content { get; init; }
    public BlockAttrsDto? Attrs { get; init; }
}

/// <summary>
/// Inline marks (bold, italic, etc.) for rich text.
/// </summary>
public record InlineMarkDto
{
    public string Type { get; init; } = string.Empty;
    public Dictionary<string, object>? Attrs { get; init; }
}

/// <summary>
/// Block attributes.
/// </summary>
public record BlockAttrsDto
{
    public int? Level { get; init; } // For headings
    public string? Language { get; init; } // For code blocks
    public string? Src { get; init; } // For images/videos
    public string? Alt { get; init; } // For images
    public string? Href { get; init; } // For links
    public string? Alignment { get; init; } // Text alignment
    public string? CalloutType { get; init; } // For callouts
}

/// <summary>
/// Article with blocks DTO.
/// </summary>
public record ArticleWithBlocksDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Status { get; init; } = string.Empty;
    public int Version { get; init; }
    public IReadOnlyList<ContentBlockDto> Blocks { get; init; } = Array.Empty<ContentBlockDto>();
    public CollaborationSessionDto? ActiveSession { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ModifiedAt { get; init; }
}

/// <summary>
/// Article editor state for frontend.
/// </summary>
public record ArticleEditorState
{
    public Guid ArticleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public IReadOnlyList<ContentBlockDto> Blocks { get; init; } = Array.Empty<ContentBlockDto>();
    public bool IsDirty { get; init; }
    public bool IsSaving { get; init; }
    public DateTime? LastSavedAt { get; init; }
    public CollaborationStateDto? Collaboration { get; init; }
}

/// <summary>
/// Collaboration state for frontend.
/// </summary>
public record CollaborationStateDto
{
    public bool IsConnected { get; init; }
    public string SessionId { get; init; } = string.Empty;
    public IReadOnlyList<ParticipantDto> Participants { get; init; } = Array.Empty<ParticipantDto>();
    public IReadOnlyList<CursorStateDto> Cursors { get; init; } = Array.Empty<CursorStateDto>();
}

/// <summary>
/// Cursor state for remote user display.
/// </summary>
public record CursorStateDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public Guid? BlockId { get; init; }
    public int? Position { get; init; }
    public SelectionRangeDto? Selection { get; init; }
}

/// <summary>
/// Selection range for highlighting.
/// </summary>
public record SelectionRangeDto
{
    public Guid StartBlockId { get; init; }
    public int StartOffset { get; init; }
    public Guid EndBlockId { get; init; }
    public int EndOffset { get; init; }
}
