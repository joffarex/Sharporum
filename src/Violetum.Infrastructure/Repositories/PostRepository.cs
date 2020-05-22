using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository, IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetPostById<TResult>(string postId, Func<Post, TResult> selector)
        {
            return _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Id == postId)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetPosts<TResult, TKey>(Func<Post, bool> condition,
            Func<Post, TResult> selector, Func<TResult, TKey> keySelector, SearchParams searchParams)
        {
            IEnumerable<TResult> query = _context.Posts
                .Include(x => x.Category)
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

        public int GetTotalPostsCount<TResult, TKey>(Func<Post, bool> condition, Func<TResult, TKey> keySelector)
        {
            return _context.Posts
                .AsEnumerable()
                .Where(condition)
                .Count();
        }


        public Task<int> CreatePost(Post post)
        {
            return CreateEntity(post);
        }

        public Task<int> UpdatePost(Post post)
        {
            return UpdateEntity(post);
        }

        public Task<int> DeletePost(Post post)
        {
            return DeleteEntity(post);
        }

        public TResult GetPostVote<TResult>(string postId, string userId, Func<PostVote, TResult> selector)
        {
            return _context.PostVotes
                .Where(x => (x.PostId == postId) && (x.UserId == userId))
                .Select(selector)
                .FirstOrDefault();
        }

        public int GetPostVoteSum(string postId)
        {
            var voteSum = _context.PostVotes
                .Where(x => x.PostId == postId)
                .GroupBy(x => x.PostId)
                .Select(x => new
                {
                    Sum = x.Sum(y => y.Direction),
                }).FirstOrDefault();

            return voteSum == null ? 0 : voteSum.Sum;
        }

        public Task<int> VotePost(PostVote postVote)
        {
            return CreateEntity(postVote);
        }

        public Task<int> UpdatePostVote(PostVote postVote)
        {
            return UpdateEntity(postVote);
        }
    }
}