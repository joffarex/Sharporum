using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Joffarex.Specification;

namespace Violetum.Infrastructure.Repositories
{
    public class BaseRepository
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        protected Task<IQueryable<TResult>> ApplySpecification<TEntity, TResult>(ISpecification<TEntity> spec,
            IConfigurationProvider configurationProvider)
            where TEntity : class
            where TResult : class
        {
            return Task.FromResult(EfSpecificationEvaluator<TEntity, TResult>.GetQuery(
                _context.Set<TEntity>().AsQueryable(), spec, configurationProvider)
            );
        }

        protected Task<IQueryable<TEntity>> ApplySpecification<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class
        {
            return Task.FromResult(
                EfSpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), spec));
        }
    }
}