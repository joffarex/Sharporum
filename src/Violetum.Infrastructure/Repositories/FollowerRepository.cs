﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public IEnumerable<TResult> GetUserFollowers<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return _context.Followers
                .Include(x => x.FollowerUser)
                .Where(x => x.UserToFollowId == userId)
                .ProjectTo<TResult>(configurationProvider)
                .ToList();
        }

        public IEnumerable<TResult> GetUserFollowing<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return _context.Followers
                .Include(x => x.UserToFollow)
                .Where(x => x.FollowerUserId == userId)
                .ProjectTo<TResult>(configurationProvider)
                .ToList();
        }

        public bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId)
        {
            return _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .FirstOrDefault(x => x.FollowerUserId == authenticatedUserId) != null;
        }

        public async Task FollowUserAsync(Follower follower)
        {
            await CreateEntityAsync(follower);
        }

        public async Task UnfollowUserAsync(string userToFollowId, string followerUserId)
        {
            Follower f = _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .FirstOrDefault(x => x.FollowerUserId == followerUserId);

            await DeleteEntityAsync(f);
        }
    }
}