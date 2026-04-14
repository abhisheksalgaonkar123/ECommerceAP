using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Events;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public string? Notes { get; private set; }

    // Private list — outside world cannot modify directly
    private readonly List<OrderItem> _orderItems = new();

    // Public readonly view — can see but cannot modify
    public IReadOnlyCollection<OrderItem> OrderItems 
        => _orderItems.AsReadOnly();

    // Calculated from items — uses Money.Add()
    public Money TotalAmount => _orderItems
        .Aggregate(
            Money.Zero("USD"),
            (sum, item) => sum.Add(item.TotalPrice));

    // EF Core uses this
    private Order() { }

    // Clean factory method — takes only what it needs
    public static Order Create(
        Guid customerId,
        Address shippingAddress,
        string? notes = null)
    {
        ArgumentNullException.ThrowIfNull(shippingAddress);

        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId cannot be empty");

        var order = new Order
        {
            CustomerId = customerId,
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Pending,
            Notes = notes
        };

        // Tell the system an order was placed!
        order.AddDomainEvent(new OrderPlacedEvent(order));

        return order;
    }

    // Instance method — called on the order itself
    public void AddItem(
        Guid productId,
        string productName,
        int quantity,
        Money unitPrice)
    {
        // If product already in order, don't add duplicate
        var existingItem = _orderItems
            .FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
            return;

        _orderItems.Add(
            OrderItem.Create(Id, productId, productName, quantity, unitPrice));
    }

    // Status transitions — business rules enforced here!
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot confirm order with status {Status}");

        Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException($"Cannot ship order with status {Status}");

        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new DomainException("Cannot cancel order that has been shipped or delivered");

        Status = OrderStatus.Cancelled;
    }
}
