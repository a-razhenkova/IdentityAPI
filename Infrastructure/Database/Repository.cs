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

        public virtual IQueryable<TEntity> Init()
            => _context.Set<TEntity>();

        public virtual async Task BasicAddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

        public virtual void BasicRemove(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);

        public virtual async Task<bool> CheckIfExistAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            => await Init().AnyAsync(expression, cancellationToken);

        public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool autoTrack = true)
        {
            IQueryable<TEntity> query = Init();

            if (!autoTrack)
                query.AsNoTracking();

            return query.Where(expression);
        }
    }
}