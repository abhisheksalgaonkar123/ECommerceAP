namespace ECommerce.Domain.ValueObjects;

public sealed class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    // Private — only way to create is through Of()
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    // Factory method — validates then creates
    public static Money Of(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        return new Money(amount, currency.ToUpper());
    }

    // Zero money — useful for order total calculations
    public static Money Zero(string currency) => Of(0, currency);

    // Returns Money not int!
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return Of(Amount + other.Amount, Currency);
    }

    // For order item total: price x quantity
    public Money Multiply(int multiplier)
        => Of(Amount * multiplier, Currency);

    // Value objects are equal if values are equal
    public override bool Equals(object? obj)
        => obj is Money other &&
           Amount == other.Amount &&
           Currency == other.Currency;

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);

    public override string ToString()
        => $"{Amount:F2} {Currency}";
}
