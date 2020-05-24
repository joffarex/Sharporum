using System.Threading.Tasks;

namespace Violetum.Domain.Infrastructure
{
    public interface IBaseRepository
    {
        Task<int> CreateEntity<TEntity>(TEntity entity) where TEntity : class;
        Task<int> UpdateEntity<TEntity>(TEntity entity) where TEntity : class;
        Task<int> DeleteEntity<TEntity>(TEntity entity) where TEntity : class;
    }
}