using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events;

public sealed class OrderPlacedEvent: BaseEvent
{
     public Order Order{ get; }
     public OrderPlacedEvent(Order order)
     => Order= order;
}
