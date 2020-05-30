using System;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICommentValidators
    {
        TResult GetCommentByIdOrThrow<TResult>(string commentId, Func<Comment, TResult> selector);
    }
}