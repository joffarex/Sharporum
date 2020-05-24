using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface ICategoryRepository
    {
        TResult GetCategory<TResult>(Expression<Func<Category, bool>> condition,
            Func<Category, TResult> selector);

        IEnumerable<TResult> GetCategories<TResult>(Func<Category, bool> condition,
            Func<Category, TResult> selector,
            CategorySearchParams searchParams);

        Task<int> CreateCategory(Category category);
        Task<int> UpdateCategory(Category category);
        Task<int> DeleteCategory(Category category);
    }
}