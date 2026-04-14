using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Simple primitives — not Money value object!
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;

    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }

    // Both Id and Name — Angular needs both!
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    // Audit info — useful for admin panel
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
