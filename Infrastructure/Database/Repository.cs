using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : EntityBase
    {
        protected readonly IdentityContext _context;

        public Repository(IdentityContext context)
        {
            _context = context;
        }

        public virtual IQueryable<TEntity> GetRepo()
            => _context.Set<TEntity>();

        public virtual async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression)
            => await _context.Set<TEntity>().AnyAsync(expression);

        public virtual async Task<TEntity?> GetByIdAsync(long id,
            Expression<Func<TEntity, object>>[]? includes = default,
            bool autoTrack = false)
        {
            return await GetQueryAsync(e => e.Id == id, includes, autoTrack).SingleOrDefaultAsync();
        }

        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? expression = default,
            Expression<Func<TEntity, object>>[]? includes = default,
            bool autoTrack = false)
        {
            return await GetQueryAsync(expression, includes, autoTrack).SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[]? includes = default, bool autoTrack = false)
            => await GetQueryAsync(default, includes, autoTrack).ToListAsync();

        protected virtual IQueryable<TEntity> GetQueryAsync(Expression<Func<TEntity, bool>>? expression = default,
            Expression<Func<TEntity, object>>[]? includes = default,
            bool autoTrack = false)
        {
            IQueryable<TEntity> query = GetRepo();

            if (expression is not null)
                query = query.Where(expression);

            if (includes is not null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (!autoTrack)
                query = query.AsNoTracking();

            return query;
        }

        public virtual async Task BasicAddAsync(TEntity entity)
            => await _context.Set<TEntity>().AddAsync(entity);

        public virtual void BasicRemove(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);
    }
}