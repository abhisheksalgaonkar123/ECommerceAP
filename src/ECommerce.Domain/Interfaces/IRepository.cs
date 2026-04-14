using System.Linq.Expressions;
using ECommerce.Domain.Common;
namespace ECommerce.Domain.Interfaces;
public interface IRepository<T> where T : BaseEntity
{// Get single by Id — nullable because might not exist
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    // Get all records
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    // Search with filter — e.g. Find(p => p.IsActive)
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Add single entity
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Add multiple entities at once
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Mark as modified — EF Core saves on SaveChanges()
    void Update(T entity);

    // Remove single
    void Remove(T entity);

    // Remove multiple
    void RemoveRange(IEnumerable<T> entities);
}