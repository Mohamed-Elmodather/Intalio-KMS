using System.Security.Claims;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Services;

/// <summary>
/// Implementation of ICurrentUser that extracts user information from HttpContext.
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("sub")?.Value
                ?? User?.FindFirst("oid")?.Value; // Azure AD object ID

            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            // If it's a GUID, parse it; otherwise generate a deterministic GUID from the string
            if (Guid.TryParse(userIdClaim, out var guid))
                return guid;

            // Generate deterministic GUID from non-GUID user identifiers
            return GenerateDeterministicGuid(userIdClaim);
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
        ?? User?.FindFirst("preferred_username")?.Value
        ?? User?.FindFirst("email")?.Value;

    public string? DisplayName => User?.FindFirst(ClaimTypes.Name)?.Value
        ?? User?.FindFirst("name")?.Value
        ?? Email;

    public string Language => User?.FindFirst("language")?.Value
        ?? User?.FindFirst("locale")?.Value?.Split('-').FirstOrDefault()
        ?? "en";

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        ?? Enumerable.Empty<string>();

    public IEnumerable<string> Permissions => User?.FindAll("permissions")
        .Select(c => c.Value)
        .Concat(User?.FindAll("permission").Select(c => c.Value) ?? Enumerable.Empty<string>())
        ?? Enumerable.Empty<string>();

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;

    public bool HasPermission(string permission) => Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);

    private static Guid GenerateDeterministicGuid(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);
        return new Guid(guidBytes);
    }
}
