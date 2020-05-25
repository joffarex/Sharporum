using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository, IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetPost<TResult>(Func<Post, bool> condition, Func<Post, TResult> selector)
        {
            return _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(condition)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetPosts<TResult, TKey>(Func<Post, bool> condition,
            Func<Post, TResult> selector, Func<TResult, TKey> keySelector, PostSearchParams searchParams)
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

        public IEnumerable<string> GetUserFollowings(string userId)
        {
            return _context.Followers
                .Where(x => x.FollowerUserId == userId)
                .Select(x => x.UserToFollowId)
                .ToList();
        }

        public int GetPostCount(Func<Post, bool> condition)
        {
            return _context.Posts.AsEnumerable().Where(condition).Count();
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

        /*public TResult GetPostVote<TResult>(string postId, string userId, Func<PostVote, TResult> selector)
        {
            return _context.PostVotes
                .Where(x => (x.PostId == postId) && (x.UserId == userId))
                .Select(selector)
                .FirstOrDefault();
                
                x => (x.CommentId == commentId) && (x.UserId == userId)
        }
        */

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
    }
}