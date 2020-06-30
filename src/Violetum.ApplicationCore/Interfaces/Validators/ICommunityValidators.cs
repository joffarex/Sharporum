using System;
using System.Linq.Expressions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICommunityValidators
    {
        TResult GetCommunityOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class;
        Community GetCommunityOrThrow(Expression<Func<Community, bool>> condition);
    }
}