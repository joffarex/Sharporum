using System.Net;
using System.Threading.Tasks;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> CreateEntity<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Can not add empty {nameof(TEntity)}");
            }

            _context.Set<TEntity>().Add(entity);

            return _context.SaveChangesAsync();
        }

        public Task<int> UpdateEntity<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Can not update empty {nameof(TEntity)}");
            }

            _context.Set<TEntity>().Update(entity);

            return _context.SaveChangesAsync();
        }

        public Task<int> DeleteEntity<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, $"Can not delete empty {nameof(TEntity)}");
            }

            _context.Set<TEntity>().Remove(entity);

            return _context.SaveChangesAsync();
        }
    }
}