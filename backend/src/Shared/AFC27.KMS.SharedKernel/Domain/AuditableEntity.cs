namespace AFC27.KMS.SharedKernel.Domain;

/// <summary>
/// Base class for entities that require audit tracking.
/// Includes creation and modification timestamps and user information.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public Guid? ModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    protected AuditableEntity() : base()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected AuditableEntity(Guid id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void SetCreatedBy(Guid userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetModifiedBy(Guid userId)
    {
        ModifiedBy = userId;
        ModifiedAt = DateTime.UtcNow;
    }

    public void SoftDelete(Guid userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}

/// <summary>
/// Auditable entity with version tracking for optimistic concurrency.
/// </summary>
public abstract class VersionedAuditableEntity : AuditableEntity
{
    public int Version { get; private set; } = 1;
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public void IncrementVersion()
    {
        Version++;
    }
}
