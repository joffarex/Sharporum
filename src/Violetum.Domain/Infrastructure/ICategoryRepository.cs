using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface ICategoryRepository
    {
        Category GetCategory(Expression<Func<Category, bool>> condition);

        TResult GetCategory<TResult>(Expression<Func<Category, bool>> condition,
            Func<Category, TResult> selector);

        IEnumerable<TResult> GetCategories<TResult>(Expression<Func<Category, bool>> condition,
            Func<Category, TResult> selector,
            Paginator paginator);

        Task<int> CreateCategory(Category category);
        Task<int> UpdateCategory(Category category);
        Task<int> DeleteCategory(Category category);
    }
}