using AirAnalysis.DAL.Data;
using AirAnalysis.DAL.Options;
using AirAnalysis.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Linq.Expressions;

namespace AirAnalysis.DAL.Repositories.Realizations
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AirAnalysisDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AirAnalysisDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (queryOptions != null)
            {
                query = ApplyQueryOptions(query, queryOptions);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (queryOptions != null)
            {
                query = ApplyTracking(query, queryOptions.AsNoTracking);
                query = ApplyInclude(query, queryOptions.Include);
                query = ApplyFilter(query, queryOptions.Filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            return (await _context.Set<T>().AddAsync(entity)).Entity;
        }

        public async Task CreateRangeAsync(params T[] entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task CreateRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public EntityEntry<T> Update(T entity)
        {
            return _context.Set<T>().Update(entity);
        }

        public void UpdateRange(params T[] entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(params T[] entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task<TKey?> MaxAsync<TKey>(
            Expression<Func<T, TKey>> selector,
            Expression<Func<T, bool>>? filter = null)
            where TKey : struct
        {
            var query = _context.Set<T>().AsNoTracking();

            query = ApplyFilter(query, filter);

            var projected = query.Select(selector);

            return await projected.DefaultIfEmpty().MaxAsync();
        }

        public async Task<int> CountAsync(QueryOptions<T>? queryOptions = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (queryOptions != null)
            {
                query = ApplyQueryOptions(query, queryOptions);
            }

            return await query.CountAsync();
        }

        private static IQueryable<T> ApplyFilter(IQueryable<T> query, Expression<Func<T, bool>>? filter)
        {
            return filter is not null ? query.Where(filter) : query;
        }

        private static IQueryable<T> ApplyInclude(IQueryable<T> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include)
        {
            return include is not null ? include(query) : query;
        }

        private static IQueryable<T> ApplyOrdering(
            IQueryable<T> query,
            Expression<Func<T, object>>? orderByASC,
            Expression<Func<T, object>>? orderByDESC)
        {
            if (orderByASC != null)
            {
                return query.OrderBy(orderByASC);
            }

            if (orderByDESC != null)
            {
                return query.OrderByDescending(orderByDESC);
            }

            return query;
        }

        private static IQueryable<T> ApplyPagination(IQueryable<T> query, int offset, int limit)
        {
            if (offset > 0)
            {
                query = query.Skip(offset);
            }

            if (limit > 0)
            {
                query = query.Take(limit);
            }

            return query;
        }

        static private IQueryable<T> ApplyTracking(IQueryable<T> query, bool asNoTracking)
        {
            return asNoTracking ? query.AsNoTracking() : query;
        }

        private static IQueryable<T> ApplyQueryOptions(IQueryable<T> query, QueryOptions<T> queryOptions)
        {
            query = ApplyTracking(query, queryOptions.AsNoTracking);
            query = ApplyInclude(query, queryOptions.Include);
            query = ApplyFilter(query, queryOptions.Filter);
            query = ApplyOrdering(query, queryOptions.OrderByASC, queryOptions.OrderByDESC);
            query = ApplyPagination(query, queryOptions.Offset, queryOptions.Limit);

            return query;
        }
    }
}
