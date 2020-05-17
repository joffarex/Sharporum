using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface ICategoryRepository
    {
        TResult GetCategoryByName<TResult>(string name, Func<Category, TResult> selector);

        IEnumerable<TResult> GetCategories<TResult>(Expression<Func<Category, bool>> predicate,
            Func<Category, TResult> selector,
            Paginator paginator);

        Task<int> CreateCategory(Category category);
        Task<int> UpdateCategory(Category category);
        Task<int> DeleteCategory(Category category);
    }
}