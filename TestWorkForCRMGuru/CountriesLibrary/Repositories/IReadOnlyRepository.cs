using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CountriesLibrary.Repositories
{
    public interface IReadOnlyRepository<TEntity, in TKey> 
        where TEntity : class, IEntity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        IQueryable<TEntity> GetAll(DbContext context);
        Task<TEntity> GetAsync(DbContext context, TKey id);
    }
}
