using System.Net;
using System.Threading.Tasks;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private readonly ApplicationDbContext _context;

        protected BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Can not add empty {nameof(TEntity)}");
            }

            await _context.Set<TEntity>().AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Can not update empty {nameof(TEntity)}");
            }

            _context.Set<TEntity>().Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Can not delete empty {nameof(TEntity)}");
            }

            _context.Set<TEntity>().Remove(entity);

            await _context.SaveChangesAsync();
        }
    }
}