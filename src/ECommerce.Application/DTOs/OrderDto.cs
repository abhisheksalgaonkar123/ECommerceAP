namespace ECommerce.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    // Status as string (NOT enum as per requirement)
    public string Status { get; set; } = string.Empty;

    // Formatted address as a single string
    public string? ShippingAddress { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}
