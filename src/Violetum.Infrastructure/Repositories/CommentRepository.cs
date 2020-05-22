using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetCommentById<TResult>(string commentId, Func<Comment, TResult> selector)
        {
            return _context.Comments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .Where(x => x.Id == commentId)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetComments<TResult, TKey>(Func<Comment, bool> condition,
            Func<Comment, TResult> selector, Func<TResult, TKey> keySelector, SearchParams searchParams)
        {
            IEnumerable<TResult> query = _context.Comments
                .Include(x => x.Author)
                .AsEnumerable()
                .Where(condition)
                .Select(selector);

            return searchParams.OrderByDir.ToUpper() == "DESC"
                ? query.OrderByDescending(keySelector)
                    .Skip(searchParams.Offset)
                    .Take(searchParams.Limit)
                    .ToList()
                : query.OrderBy(keySelector)
                    .Skip(searchParams.Offset)
                    .Take(searchParams.Limit)
                    .ToList();
        }

        public int GetTotalCommentsCount<TResult, TKey>(Func<Comment, bool> condition, Func<TResult, TKey> keySelector)
        {
            return _context.Comments
                .AsEnumerable()
                .Where(condition)
                .Count();
        }

        public Task<int> CreateComment(Comment comment)
        {
            return CreateEntity(comment);
        }

        public Task<int> UpdateComment(Comment comment)
        {
            return UpdateEntity(comment);
        }

        public Task<int> DeleteComment(Comment comment)
        {
            return DeleteEntity(comment);
        }

        public TResult GetCommentVote<TResult>(string commentId, string userId, Func<CommentVote, TResult> selector)
        {
            return _context.CommentVotes
                .Where(x => (x.CommentId == commentId) && (x.UserId == userId))
                .Select(selector)
                .FirstOrDefault();
        }

        public int GetCommentVoteSum(string commentId)
        {
            var voteSum = _context.CommentVotes
                .Where(x => x.CommentId == commentId)
                .GroupBy(x => x.CommentId)
                .Select(x => new
                {
                    Sum = x.Sum(y => y.Direction),
                }).FirstOrDefault();

            return voteSum == null ? 0 : voteSum.Sum;
        }

        public Task<int> VoteComment(CommentVote commentVote)
        {
            return CreateEntity(commentVote);
        }

        public Task<int> UpdateCommentVote(CommentVote commentVote)
        {
            return UpdateEntity(commentVote);
        }
    }
}