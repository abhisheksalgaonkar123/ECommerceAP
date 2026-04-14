using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Events;

namespace ECommerce.Domain.Entities;

public class ApplicationUser : BaseAuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    // Calculated — not stored in DB
    public string FullName => $"{FirstName} {LastName}";

    // EF Core uses this
    private ApplicationUser() { }

    public static ApplicationUser Create(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role = UserRole.Customer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var user = new ApplicationUser
        {

            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true
        };

        user.AddDomainEvent(new UserRegisteredEvent(user));
        return user;
    }

    public void SetRefreshToken(string token, DateTime expiry)
    {
        RefreshToken = token;
        RefreshTokenExpiry = expiry;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
    }
}
