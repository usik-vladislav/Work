using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CountriesLibrary.Repositories
{
    public class ReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TKey : IEquatable<TKey> 
    {
        public IQueryable<TEntity> GetAll(DbContext context)
        {
            return context.Set<TEntity>().AsNoTracking();
        }

        public async Task<TEntity> GetAsync(DbContext context, TKey id)
        {
            return await context.FindAsync<TEntity>(id);
        }
    }
}
