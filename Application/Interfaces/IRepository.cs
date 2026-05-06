using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> GetRepo();

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity?> GetByIdAsync(long id,
            Expression<Func<TEntity, object>>[]? includes = default,
            bool autoTrack = false);

        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? expression = default,
            Expression<Func<TEntity, object>>[]? includes = default,
            bool autoTrack = false);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[]? includes = default, bool autoTrack = false);

        Task BasicAddAsync(TEntity entity);

        void BasicRemove(TEntity entity);
    }
}