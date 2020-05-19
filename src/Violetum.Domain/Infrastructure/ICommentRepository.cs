using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface ICommentRepository
    {
        TResult GetCommentById<TResult>(string commentId, Func<Comment, TResult> selector);

        IEnumerable<TResult> GetComments<TResult>(Func<Comment, bool> condition,
            Func<Comment, TResult> selector,
            Paginator paginator);

        Task<int> CreateComment(Comment comment);
        Task<int> UpdateComment(Comment comment);
        Task<int> DeleteComment(Comment comment);
    }
}