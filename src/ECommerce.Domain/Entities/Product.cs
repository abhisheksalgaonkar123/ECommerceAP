
using ECommerce.Domain.Common;
using ECommerce.Domain.Events;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid CategoryId { get; private set; }

    // EF Core uses this to read from database
    public Product() { }

    public static Product Create(
        string name,
        string description,
        decimal price,
        string currency,
        int stockQuantity,
        Guid categoryId,
        string? imageUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative");

        var product = new Product
        {
            Name = name,
            Description = description,
            Price = Money.Of(price, currency), // Money validates price!
            StockQuantity = stockQuantity,
            CategoryId = categoryId,
            ImageUrl = imageUrl,
            IsActive = true
        };

        // Tell the rest of the system a product was created
        product.AddDomainEvent(new ProductCreatedEvent(product));

        return product;
    }

    // Update basic details
    public void UpdateDetails(string name, string description, string? imageUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
    }

    // Update price — Money validates amount
    public void UpdatePrice(decimal amount, string currency)
        => Price = Money.Of(amount, currency);

    // Add stock — must be positive
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to add must be positive");
        StockQuantity += quantity;
    }

    // Deduct stock — uses our custom exception!
    public void DeductStock(int quantity)
    {
        if (!HasSufficientStock(quantity))
            throw new InsufficientStockException(Name, quantity, StockQuantity);
        StockQuantity -= quantity;
    }

    // Check before deducting
    public bool HasSufficientStock(int quantity)
        => StockQuantity >= quantity;

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
