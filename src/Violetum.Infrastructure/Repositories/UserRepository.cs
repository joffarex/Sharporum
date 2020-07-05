using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;

namespace Violetum.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TResult>> ListUserFollowersAsync<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Followers
                .Include(x => x.FollowerUser)
                .Where(x => x.UserToFollowId == userId)
                .ProjectTo<TResult>(configurationProvider)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListUserFollowingAsync<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Followers
                .Include(x => x.UserToFollow)
                .Where(x => x.FollowerUserId == userId)
                .ProjectTo<TResult>(configurationProvider)
                .ToListAsync();
        }

        public async Task<bool> IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId)
        {
            return await _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .FirstOrDefaultAsync(x => x.FollowerUserId == authenticatedUserId) != null;
        }

        public async Task FollowUserAsync(Follower follower)
        {
            await _context.Followers.AddAsync(follower);

            await _context.SaveChangesAsync();
        }

        public async Task UnfollowUserAsync(string userToFollowId, string followerUserId)
        {
            Follower f = await _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .FirstOrDefaultAsync(x => x.FollowerUserId == followerUserId);

            _context.Followers.Remove(f);

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<string>> ListUserFollowingsAsync(string userId)
        {
            return await _context.Followers
                .Where(x => x.FollowerUserId == userId)
                .Select(x => x.UserToFollowId)
                .ToListAsync();
        }

        public async Task<double> GetUserPostRank(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<double> GetUserCommentRank(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Ranks>> ListRanks<TRank>()
        {
            throw new NotImplementedException();
        }
    }
}