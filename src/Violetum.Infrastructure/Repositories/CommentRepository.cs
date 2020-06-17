using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;
using Violetum.Infrastructure.Extensions;

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

        public TResult GetComment<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return _context.Comments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefault();
        }

        public Comment GetComment(Expression<Func<Comment, bool>> condition)
        {
            return _context.Comments
                .Include(x => x.Post)
                .Include(x => x.Author)
                .Where(condition)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetComments<TResult>(CommentSearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Comment, ICollection<CommentVote>> query = _context.Comments
                .Include(x => x.Author)
                .Include(x => x.CommentVotes);

            IQueryable<Comment> whereParams = WhereConditionPredicate(query, searchParams);

            IOrderedQueryable<TResult> orderedQuery = searchParams.OrderByDir.ToUpper() == "DESC"
                ? whereParams
                    .ProjectTo<TResult>(configurationProvider)
                    .OrderByDescending(searchParams.SortBy)
                : whereParams
                    .ProjectTo<TResult>(configurationProvider)
                    .OrderBy(searchParams.SortBy);
            return orderedQuery
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .ToList();
        }

        public int GetCommentsCount(CommentSearchParams searchParams)
        {
            DbSet<Comment> query = _context.Comments;
            IQueryable<Comment> whereParams = WhereConditionPredicate(query, searchParams);

            return whereParams.Count();
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

        public List<Ranks> GetCommentRanks()
        {
            List<Ranks> userEntityVotes = _context.CommentVotes
                .GroupBy(x => x.UserId, (key, vote) =>
                    new Ranks
                    {
                        UserId = key,
                        Rank = vote.Sum(x => x.Direction),
                    })
                .ToList();

            return userEntityVotes;
        }

        public int GetUserCommentRank(string userId)
        {
            return _context.CommentVotes
                .GroupBy(x => x.UserId, (key, vote) => vote.Sum(x => x.Direction))
                .FirstOrDefault();
        }

        private static IQueryable<Comment> WhereConditionPredicate(IQueryable<Comment> query,
            CommentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                query = query.Where(x => x.AuthorId == searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.PostId))
            {
                query = query.Where(x => x.PostId == searchParams.PostId);
            }

            return query;
        }
    }
}