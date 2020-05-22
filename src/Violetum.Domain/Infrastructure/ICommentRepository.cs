using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface ICommentRepository
    {
        TResult GetCommentById<TResult>(string commentId, Func<Comment, TResult> selector);

        IEnumerable<TResult> GetComments<TResult, TKey>(Func<Comment, bool> condition,
            Func<Comment, TResult> selector, Func<TResult, TKey> keySelector, SearchParams searchParams);

        int GetTotalCommentsCount<TResult, TKey>(Func<Comment, bool> condition, Func<TResult, TKey> keySelector);

        Task<int> CreateComment(Comment comment);
        Task<int> UpdateComment(Comment comment);
        Task<int> DeleteComment(Comment comment);

        TResult GetCommentVote<TResult>(string commentId, string userId, Func<CommentVote, TResult> selector);
        int GetCommentVoteSum(string commentId);
        Task<int> VoteComment(CommentVote commentVote);
        Task<int> UpdateCommentVote(CommentVote commentVote);
    }
}