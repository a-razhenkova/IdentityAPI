using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : EntityBase
    {
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(IdentityContext context)
        {
            _dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get()
        {
            return _dbSet;
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(expression, cancellationToken);
        }

        public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool autoTrack = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!autoTrack)
                query.AsNoTracking();

            return query.Where(expression);
        }
    }
}