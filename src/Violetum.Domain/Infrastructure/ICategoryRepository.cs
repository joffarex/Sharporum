using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface ICategoryRepository
    {
        TResult GetCategory<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider)
            where TResult : class;

        Category GetCategory(Expression<Func<Category, bool>> condition);

        IEnumerable<TResult> GetCategories<TResult>(CategorySearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class;

        int GetCategoryCount(CategorySearchParams searchParams);

        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
}