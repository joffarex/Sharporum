using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetComment<TResult>(Func<Comment, bool> condition, Func<Comment, TResult> selector)
        {
            return _context.Comments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .Where(condition)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetComments<TResult, TKey>(Func<Comment, bool> condition,
            Func<Comment, TResult> selector, Func<TResult, TKey> keySelector, CommentSearchParams searchParams)
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

        public int GetTotalCommentsCount(Func<Comment, bool> condition)
        {
            return _context.Comments.AsEnumerable().Where(condition).Count();
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
            IIncludableQueryable<Comment, IEnumerable<CommentVote>> votes =
                _context.Comments.Where(x => x.Id == comment.Id).Include(x => x.CommentVotes);
            _context.Comments.RemoveRange(votes);

            return DeleteEntity(comment);
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
    }
}