using System.Threading.Tasks;

namespace Violetum.Domain.Infrastructure
{
    public interface IBaseRepository
    {
        Task CreateEntityAsync<TEntity>(TEntity entity) where TEntity : class;
        Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : class;
    }
}