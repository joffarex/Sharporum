using System;
using System.Linq.Expressions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface IPostValidators
    {
        TResult GetPostOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
        Post GetPostOrThrow(Expression<Func<Post, bool>> condition);
    }
}