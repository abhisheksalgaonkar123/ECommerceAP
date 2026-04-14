using System;

namespace ECommerce.Domain.Exceptions;

public class InsufficientStockException(string productName, int requested, int available) : DomainException($"Insufficient stock for '{productName}'. Requested: {requested}, Available: {available}")
{
    // Read only properties — exception data never changes
    public string ProductName { get; } = productName;
    public int Requested { get; } = requested;
    public int Available { get; } = available;
}
