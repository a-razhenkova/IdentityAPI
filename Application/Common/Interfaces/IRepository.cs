using System.Linq.Expressions;

namespace Application
{
    public interface IRepository<TEntity>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Remove(TEntity entity);

        IRepository<TEntity> Where(Expression<Func<TEntity, bool>> expression);
        IRepository<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationPropertyPath);
        IRepository<TEntity> AsNoTracking();
        IQueryable<TEntity> ToQuery();

        Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
    }
}