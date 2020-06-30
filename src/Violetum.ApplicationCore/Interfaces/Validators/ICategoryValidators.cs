using System;
using System.Linq.Expressions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICategoryValidators
    {
        TResult GetCategoryOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
        Category GetCategoryOrThrow(Expression<Func<Category, bool>> condition);
    }
}