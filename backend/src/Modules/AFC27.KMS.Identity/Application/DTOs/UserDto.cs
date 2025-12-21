namespace AFC27.KMS.Identity.Application.DTOs;

/// <summary>
/// User response DTO.
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? AvatarUrl { get; init; }
    public string? JobTitle { get; init; }
    public string? JobTitleArabic { get; init; }
    public string? PhoneNumber { get; init; }
    public string? EmployeeId { get; init; }
    public string PreferredLanguage { get; init; } = "en";
    public bool IsActive { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public Guid? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// User profile DTO for display.
/// </summary>
public record UserProfileDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? AvatarUrl { get; init; }
    public string? JobTitle { get; init; }
    public string? JobTitleArabic { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Bio { get; init; }
    public string? Location { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = Array.Empty<string>();
    public DepartmentSummaryDto? Department { get; init; }
    public UserSummaryDto? Manager { get; init; }
    public IReadOnlyList<UserSummaryDto> DirectReports { get; init; } = Array.Empty<UserSummaryDto>();
}

/// <summary>
/// Lightweight user summary for lists and references.
/// </summary>
public record UserSummaryDto
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string? JobTitle { get; init; }
}

/// <summary>
/// Create user request.
/// </summary>
public record CreateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? JobTitle { get; init; }
    public string? JobTitleArabic { get; init; }
    public string? PhoneNumber { get; init; }
    public string? EmployeeId { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? ManagerId { get; init; }
    public IReadOnlyList<Guid>? RoleIds { get; init; }
}

/// <summary>
/// Update user request.
/// </summary>
public record UpdateUserRequest
{
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? JobTitle { get; init; }
    public string? JobTitleArabic { get; init; }
    public string? PhoneNumber { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? ManagerId { get; init; }
}

/// <summary>
/// Update profile request (self-service).
/// </summary>
public record UpdateProfileRequest
{
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? PhoneNumber { get; init; }
    public string PreferredLanguage { get; init; } = "en";
    public string? Bio { get; init; }
    public string? Location { get; init; }
    public IReadOnlyList<string>? Skills { get; init; }
}

/// <summary>
/// User filter for queries.
/// </summary>
public record UserFilterRequest
{
    public string? SearchTerm { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? RoleId { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
}
