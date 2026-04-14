using System;

namespace ECommerce.Domain.ValueObjects;

public sealed class Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }
    public string ZipCode { get; }

    private Address(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public static Address Of(string street, string city, string state, string country, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty", nameof(country));
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode cannot be null or empty", nameof(zipCode));

        return new Address(street.Trim(), city.Trim(), state?.Trim() ?? string.Empty , country.Trim(), zipCode.Trim());
    }

    public override bool Equals(object? obj)
        => obj is Address other &&
           Street == other.Street &&
           City == other.City &&
           State == other.State &&
           Country == other.Country &&
           ZipCode == other.ZipCode;

    public override int GetHashCode()
        => HashCode.Combine(Street, City, State, Country, ZipCode);

    public override string ToString()
        => $"{Street}, {City}, {State} {ZipCode}, {Country}";
}
