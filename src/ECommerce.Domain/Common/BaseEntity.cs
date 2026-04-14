
namespace ECommerce.Domain.Common;
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public readonly List<BaseEvent> _domainEvents = new();
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)    
       =>  _domainEvents.Add(domainEvent);
    
    public void ClearDomainEvents()
    => _domainEvents.Clear();

}