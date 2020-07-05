using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Joffarex.Specification;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface IAsyncRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class;

        Task<TEntity> GetByConditionAsync(Expression<Func<TEntity, bool>> condition);

        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity> specification,
            IConfigurationProvider configurationProvider) where TResult : class;

        Task<int> GetTotalCountAsync(ISpecification<TEntity> specification);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}