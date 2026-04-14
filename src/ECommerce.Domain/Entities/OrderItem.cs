using ECommerce.Domain.Common;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;

    // Calculated — not stored in DB
    // reuses Money.Multiply() we built!
    public Money TotalPrice => UnitPrice.Multiply(Quantity);

    // EF Core uses this
    private OrderItem() { }

    public static OrderItem Create(
        Guid orderId,
        Guid productId,
        string productName,
        int quantity,
        Money unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentNullException.ThrowIfNull(unitPrice);

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
