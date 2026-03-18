namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Active editor response DTO (Phase 3D).
/// </summary>
public record ActiveEditorDto
{
    public Guid Id { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserAvatarUrl { get; init; }
    public string Color { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public Guid? FocusedBlockId { get; init; }
    public bool IsTyping { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime LastActivityAt { get; init; }
}

/// <summary>
/// Summary of active editors for a content item.
/// </summary>
public record ContentEditorsDto
{
    public Guid ContentId { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public int EditorCount { get; init; }
    public IReadOnlyList<ActiveEditorDto> Editors { get; init; } = Array.Empty<ActiveEditorDto>();
}

/// <summary>
/// Request to register as an active editor.
/// </summary>
public record RegisterEditorRequest
{
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
}

/// <summary>
/// Request to update editor focus.
/// </summary>
public record UpdateEditorFocusRequest
{
    public Guid? FocusedBlockId { get; init; }
    public bool IsTyping { get; init; }
}

/// <summary>
/// Presence indicator for content list views - shows which items have active editors.
/// </summary>
public record ContentPresenceIndicator
{
    public Guid ContentId { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public int ActiveEditorCount { get; init; }
    public IReadOnlyList<EditorBadgeDto> Editors { get; init; } = Array.Empty<EditorBadgeDto>();
}

/// <summary>
/// Minimal editor info for badge display.
/// </summary>
public record EditorBadgeDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string Color { get; init; } = string.Empty;
}
