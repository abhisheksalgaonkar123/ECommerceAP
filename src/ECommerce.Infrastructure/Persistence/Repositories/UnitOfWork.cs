using ECommerce.Domain.Common;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class UnitOfWork:IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public  IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(typeof(T)))
        {
            var repositoryType = new Repository<T>(_context);
            _repositories.Add(type, repositoryType);
            return repositoryType;
        }
        return (_repositories[type] as IRepository<T>)!;;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    public void Dispose()
    {
       _context.Dispose();
    }
}
