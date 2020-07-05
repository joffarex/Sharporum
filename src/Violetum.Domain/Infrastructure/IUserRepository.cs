using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;

namespace Violetum.Domain.Infrastructure
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<TResult>> ListUserFollowersAsync<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class;

        Task<IReadOnlyList<TResult>> ListUserFollowingAsync<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class;

        Task<bool> IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task FollowUserAsync(Follower follower);
        Task UnfollowUserAsync(string userToFollowId, string followerUserId);
        Task<IReadOnlyList<string>> ListUserFollowingsAsync(string userId);
        Task<double> GetUserRank<TVoteEntity>(string userId) where TVoteEntity : BaseVoteEntity;

        Task<IReadOnlyList<Ranks>> ListRanks<TVoteEntity>() where TVoteEntity : BaseVoteEntity;
    }
}