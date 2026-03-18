using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Enhanced audit trail query request with rich filtering and export options.
/// </summary>
public record AuditTrailQueryRequest
{
    // Time range
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }

    // Entity filters
    public string? Action { get; init; }
    public string? Category { get; init; }
    public string? EntityType { get; init; }
    public Guid? EntityId { get; init; }

    // User filters
    public Guid? UserId { get; init; }
    public string? UserEmail { get; init; }
    public string? UserName { get; init; }

    // Severity & status
    public AuditSeverity? MinSeverity { get; init; }
    public AuditSeverity? MaxSeverity { get; init; }
    public bool? IsSuccess { get; init; }

    // Text search
    public string? SearchText { get; init; }

    // Network filters
    public string? IpAddress { get; init; }
    public string? CorrelationId { get; init; }
    public string? SessionId { get; init; }

    // Pagination & sorting
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public string SortBy { get; init; } = "Timestamp";
    public bool SortDescending { get; init; } = true;

    // Export
    public string? ExportFormat { get; init; } // "csv", "json", "pdf"
}

/// <summary>
/// Detailed audit trail entry DTO with full context.
/// </summary>
public record AuditTrailEntryDto
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public AuditSeverity Severity { get; init; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? AdditionalData { get; init; }
    public string? CorrelationId { get; init; }
    public string? SessionId { get; init; }
}

/// <summary>
/// Audit trail summary with aggregations.
/// </summary>
public record AuditTrailSummaryDto
{
    public int TotalEntries { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public int CriticalCount { get; init; }
    public Dictionary<string, int> ByCategory { get; init; } = new();
    public Dictionary<string, int> ByAction { get; init; } = new();
    public Dictionary<string, int> BySeverity { get; init; } = new();
    public Dictionary<string, int> ByEntityType { get; init; } = new();
    public IReadOnlyList<AuditTimelinePointDto> Timeline { get; init; } = Array.Empty<AuditTimelinePointDto>();
    public IReadOnlyList<AuditUserSummaryDto> TopUsers { get; init; } = Array.Empty<AuditUserSummaryDto>();
}

public record AuditTimelinePointDto
{
    public DateTime Timestamp { get; init; }
    public int Count { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
}

public record AuditUserSummaryDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserEmail { get; init; }
    public int ActionCount { get; init; }
    public int FailureCount { get; init; }
    public DateTime LastActivity { get; init; }
}

/// <summary>
/// Audit export result.
/// </summary>
public record AuditExportResultDto
{
    public string Format { get; init; } = string.Empty;
    public byte[] Data { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public int RecordCount { get; init; }
}
