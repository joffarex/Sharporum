using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface IBaseRepository
    {
        IEnumerable<TResult> GetEntities<TEntity, TResult>(Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TResult> selector, Paginator paginator) where TEntity : class;

        Task<int> CreateEntity<TEntity>(TEntity entity) where TEntity : class;
        Task<int> UpdateEntity<TEntity>(TEntity entity) where TEntity : class;
        Task<int> DeleteEntity<TEntity>(TEntity entity) where TEntity : class;
    }
}