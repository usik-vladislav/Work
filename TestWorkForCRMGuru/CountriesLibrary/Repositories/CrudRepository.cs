using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace CountriesLibrary.Repositories
{
    public class CrudRepository<TEntity, TKey> : ReadOnlyRepository<TEntity,TKey>, ICrudRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        public async Task AddAsync(DbContext context, TEntity entity)
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DbContext context, TKey id)
        {
            await context.Set<TEntity>().Where(item => item.Id.Equals(id)).DeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DbContext context, TKey id, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            await context.Set<TEntity>().Where(item => item.Id.Equals(id)).UpdateAsync(updateFactory);
            await context.SaveChangesAsync();
        }
    }
}
