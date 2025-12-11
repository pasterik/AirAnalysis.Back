using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Options;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace AirAnalysis.DAL.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null);
        Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null);
        Task<T> CreateAsync(T entity);
        Task CreateRangeAsync(params T[] entities);
        Task CreateRangeAsync(IEnumerable<T> entities);
        EntityEntry<T> Update(T entity);
        void UpdateRange(params T[] entities);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(params T[] entities);
        void DeleteRange(IEnumerable<T> entities);
        Task<TKey?> MaxAsync<TKey>(Expression<Func<T, TKey>> selector, Expression<Func<T, bool>>? filter = null) where TKey : struct;
        Task<int> CountAsync(QueryOptions<T>? queryOptions = null);
    }
}
