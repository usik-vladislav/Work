using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CountriesLibrary.Repositories
{
    public interface ICrudRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task AddAsync(DbContext context, TEntity entity);
        Task DeleteAsync(DbContext context, TKey id);
        Task UpdateAsync(DbContext context, TKey id, Expression<Func<TEntity, TEntity>> updateFactory);
    }
}
