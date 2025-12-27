using AISEP.Domain.Entities;
using System.Linq.Expressions;

namespace AISEP.Domain.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations
/// Follows Repository Pattern to abstract data access
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    // Query operations
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    // Pagination
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default);

    // Command operations
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    // Soft delete
    Task SoftDeleteAsync(T entity, CancellationToken cancellationToken = default);

    // Unit of Work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
