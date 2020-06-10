using System;
using System.Linq.Expressions;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface IPostValidators
    {
        TResult GetPostOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
    }
}