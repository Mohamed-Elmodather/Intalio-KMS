using MediatR;

namespace AFC27.KMS.SharedKernel.Domain;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain
/// and are used to communicate between aggregates and modules.
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

/// <summary>
/// Base implementation of domain event with common properties.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
