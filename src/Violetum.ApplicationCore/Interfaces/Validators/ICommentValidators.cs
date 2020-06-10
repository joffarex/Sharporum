using System;
using System.Linq.Expressions;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICommentValidators
    {
        TResult GetCommentOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
    }
}