﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface IFollowerRepository
    {
        IEnumerable<TResult> GetUserFollowers<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class;

        IEnumerable<TResult> GetUserFollowing<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class;

        bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task<int> FollowUser(Follower follower);
        Task<int> UnfollowUser(string userToFollowId, string followerUserId);
    }
}