using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface ICommentRepository
    {
        TResult GetComment<TResult>(Func<Comment, bool> condition, Func<Comment, TResult> selector);

        IEnumerable<TResult> GetComments<TResult, TKey>(Func<Comment, bool> condition,
            Func<Comment, TResult> selector, Func<TResult, TKey> keySelector, CommentSearchParams searchParams);

        int GetTotalCommentsCount(Func<Comment, bool> condition);

        Task<int> CreateComment(Comment comment);
        Task<int> UpdateComment(Comment comment);
        Task<int> DeleteComment(Comment comment);

        int GetCommentVoteSum(string commentId);
    }
}