using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;

namespace Violetum.Domain.Infrastructure
{
    public interface IAsyncRepository<TEntity>
    {
        Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class;

        Task<TEntity> GetByConditionAsync(Expression<Func<TEntity, bool>> condition);

        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISearchParams<TEntity> searchParams,
            IConfigurationProvider configurationProvider) where TResult : class;

        Task<int> GetTotalCountAsync(ISearchParams<TEntity> searchParams);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}