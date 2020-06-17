using System;
using System.Linq.Expressions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICommentValidators
    {
        TResult GetCommentOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
        Comment GetCommentOrThrow(Expression<Func<Comment, bool>> condition);
    }
}