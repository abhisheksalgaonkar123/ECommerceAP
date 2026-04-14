using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events;

public sealed class ProductCreatedEvent : BaseEvent
{
    public Product Product{ get;}
    public ProductCreatedEvent(Product product)
    => Product = product;
    
}