using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tasky.Application.Abstractions;
using Tasky.Infrastructure.Persistence;

namespace Tasky.Infrastructure.Repositories;

public sealed class EfRepository<T>(AppDbContext db) : IRepository<T> where T : class
{
    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Set<T>().FindAsync([id], ct).AsTask();

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        IQueryable<T> q = db.Set<T>();
        if (predicate is not null) q = q.Where(predicate);
        return await q.ToListAsync(ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        db.Set<T>().Add(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        db.Set<T>().Update(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        db.Set<T>().Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
