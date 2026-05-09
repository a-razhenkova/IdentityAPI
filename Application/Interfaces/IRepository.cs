using System.Linq.Expressions;

namespace Application
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Init();

        Task BasicAddAsync(TEntity entity, CancellationToken cancellationToken = default);

        void BasicRemove(TEntity entity);

        Task<bool> CheckIfExistAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression, bool autoTrack = true);
    }
}