using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface IFollowerRepository
    {
        IEnumerable<TResult> GetUserFollowers<TResult>(string userId, Func<Follower, TResult> selector);
        IEnumerable<TResult> GetUserFollowing<TResult>(string userId, Func<Follower, TResult> selector);

        bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task<int> FollowUser(Follower follower);
        Task<int> UnfollowUser(string userToFollowId, string followerUserId);
    }
}