using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// Records each verification event for an article, providing a full audit trail
/// of the knowledge verification lifecycle.
/// </summary>
public class VerificationRecord : AuditableEntity
{
    public Guid ArticleId { get; private set; }
    public Guid VerifiedById { get; private set; }
    public string VerifiedByName { get; private set; } = string.Empty;
    public DateTime VerifiedAt { get; private set; }
    public string? Notes { get; private set; }
    public VerificationStatus PreviousStatus { get; private set; }
    public VerificationStatus NewStatus { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    // Navigation properties
    public virtual Article? Article { get; private set; }

    private VerificationRecord() { }

    public static VerificationRecord Create(
        Guid articleId,
        Guid verifiedById,
        string verifiedByName,
        VerificationStatus previousStatus,
        VerificationStatus newStatus,
        DateTime? expiresAt,
        string? notes = null)
    {
        return new VerificationRecord
        {
            ArticleId = articleId,
            VerifiedById = verifiedById,
            VerifiedByName = verifiedByName,
            VerifiedAt = DateTime.UtcNow,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ExpiresAt = expiresAt,
            Notes = notes
        };
    }
}
