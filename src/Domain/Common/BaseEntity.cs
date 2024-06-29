using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFridgeManagerAPI.Domain.Common;

public abstract class BaseEntity
{
    private readonly List<BaseEvent> _domainEvents = new();
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifyAt { get; set; }

    [NotMapped] public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
