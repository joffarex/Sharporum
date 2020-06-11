using System;
using System.Linq.Expressions;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICategoryValidators
    {
        TResult GetCategoryOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
    }
}