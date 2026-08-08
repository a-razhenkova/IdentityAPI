using System.Linq.Expressions;

namespace Application
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Get();

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Remove(TEntity entity);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool autoTrack = true);
    }
}