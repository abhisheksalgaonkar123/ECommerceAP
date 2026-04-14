using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Private — EF Core uses this to read from database
    private Category() { }

    // Static factory — developers use this to create new category
    public static Category Create(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Category
        {
            Name = name,
            Description = description
        };
    }

    // Update existing category details
    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
    }
}
