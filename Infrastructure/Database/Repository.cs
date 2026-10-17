using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Registry;
using Shared;
using System.Linq.Expressions;

namespace Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : EntityBase
    {
        protected readonly DbSet<TEntity> _dbSet;
        private readonly ResiliencePipeline _pipeline;

        public Repository(IdentityContext context, ResiliencePipelineProvider<ResiliencePipelineType> pipelineProvider)
        {
            _dbSet = context.Set<TEntity>();
            _pipeline = pipelineProvider.GetPipeline(ResiliencePipelineType.DbContext);

            Query = _dbSet;
        }

        protected IQueryable<TEntity> Query { get; set; }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual IRepository<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            Query = _dbSet.Where(expression);
            return this;
        }

        public virtual IRepository<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        {
            Query = Query.Include(navigationPropertyPath);
            return this;
        }

        public virtual IRepository<TEntity> AsNoTracking()
        {
            Query = Query.AsNoTracking();
            return this;
        }

        public virtual IQueryable<TEntity> ToQuery()
        {
            return Query;
        }

        public virtual async Task<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _pipeline.ExecuteAsync(async (ct) => await Query.SingleOrDefaultAsync(ct), cancellationToken);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _pipeline.ExecuteAsync(async (ct) => await Query.FirstOrDefaultAsync(ct), cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _pipeline.ExecuteAsync(async (ct) => await _dbSet.AnyAsync(expression, ct), cancellationToken);
        }
    }
}