using System.Reflection;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence;

public class ApplicationDbContext:DbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }
    public DbSet<Product> Products{ get; set; }
    public DbSet<Category> Categories{ get; set; }
    public DbSet<Order> Orders{ get; set; }
    public DbSet<OrderItem> OrderItems{ get; set; }
    public DbSet<ApplicationUser> Users{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Ignore domain events — not database tables!
        modelBuilder.Ignore<BaseEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // 1. Auto populate audit fields
        var entries = ChangeTracker
            .Entries<BaseAuditableEntity>()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
        }

        // 2. Save to database
        var result = await base.SaveChangesAsync(cancellationToken);

        // 3. Dispatch domain events AFTER successful save
        await DispatchDomainEvents();

        return result;
    }

    private async Task DispatchDomainEvents()
    {
        // Find all entities that have raised events
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear BEFORE dispatching!
        // Prevents events firing twice if save called again
        entities.ForEach(e => e.ClearDomainEvents());

        // Publish each event — handlers react automatically
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }

}
