namespace AFC27.KMS.Identity.Application.DTOs;

/// <summary>
/// Login request.
/// </summary>
public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// SSO callback request.
/// </summary>
public record SSOCallbackRequest
{
    public string Code { get; init; } = string.Empty;
    public string? State { get; init; }
}

/// <summary>
/// Token refresh request.
/// </summary>
public record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// Change password request.
/// </summary>
public record ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

/// <summary>
/// Authentication response.
/// </summary>
public record AuthResponse
{
    public UserDto User { get; init; } = null!;
    public TokensDto Tokens { get; init; } = null!;
}

/// <summary>
/// JWT tokens.
/// </summary>
public record TokensDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

/// <summary>
/// SSO configuration response.
/// </summary>
public record SSOConfigResponse
{
    public string AuthUrl { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
    public string Scope { get; init; } = string.Empty;
}
