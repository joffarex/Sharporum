using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class FollowerRepository : BaseRepository, IFollowerRepository
    {
        private readonly ApplicationDbContext _context;

        public FollowerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<TResult> GetUserFollowers<TResult>(string userId, Func<Follower, TResult> selector)
        {
            return _context.Followers
                .Include(x => x.FollowerUser)
                .AsEnumerable()
                .Where(x => x.UserToFollowId == userId)
                .Select(selector)
                .ToList();
        }

        public IEnumerable<TResult> GetUserFollowing<TResult>(string userId, Func<Follower, TResult> selector)
        {
            return _context.Followers
                .Include(x => x.UserToFollow)
                .AsEnumerable()
                .Where(x => x.FollowerUserId == userId)
                .Select(selector)
                .ToList();
        }

        public bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId)
        {
            return _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .Where(x => x.FollowerUserId == authenticatedUserId)
                .FirstOrDefault() != null;
        }

        public Task<int> FollowUser(Follower follower)
        {
            return CreateEntity(follower);
        }

        public Task<int> UnfollowUser(string userToFollowId, string followerUserId)
        {
            Follower f = _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .Where(x => x.FollowerUserId == followerUserId)
                .FirstOrDefault();

            return DeleteEntity(f);
        }
    }
}