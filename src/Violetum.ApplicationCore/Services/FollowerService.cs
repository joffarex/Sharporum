using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class FollowerService : IFollowerService
    {
        private readonly IFollowerRepository _followerRepository;
        private readonly UserManager<User> _userManager;

        public FollowerService(IFollowerRepository followerRepository, UserManager<User> userManager)
        {
            _followerRepository = followerRepository;
            _userManager = userManager;
        }

        public async Task<UserFollowersViewModel> GetUserFollowersAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            IEnumerable<FollowerViewModel> userFollowers =
                _followerRepository.GetUserFollowing<FollowerViewModel>(userId,
                    UserHelpers.GetFollowerMapperConfiguration());

            return new UserFollowersViewModel
            {
                UserId = user.Id,
                Followers = userFollowers,
            };
        }

        public async Task<UserFollowingViewModel> GetUserFollowingAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            IEnumerable<FollowingViewModel> userFollowings =
                _followerRepository.GetUserFollowing<FollowingViewModel>(userId,
                    UserHelpers.GetFollowerMapperConfiguration());

            return new UserFollowingViewModel
            {
                UserId = user.Id,
                Followings = userFollowings,
            };
        }

        public bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId)
        {
            return _followerRepository.IsAuthenticatedUserFollower(userToFollowId, authenticatedUserId);
        }

        public async Task FollowUserAsync(string userId, string userToFollowId)
        {
            User followerUser = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(followerUser, nameof(followerUser));

            User userToFollow = await _userManager.FindByIdAsync(userToFollowId);
            Guard.Against.NullItem(userToFollow, nameof(userToFollow));

            var follower = new Follower
            {
                UserToFollowId = userToFollow.Id,
                FollowerUserId = followerUser.Id,
            };

            await _followerRepository.FollowUserAsync(follower);
        }

        public async Task UnfollowUserAsync(string userId, string userToUnfollowId)
        {
            User followerUser = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(followerUser, nameof(followerUser));

            User userToUnfollow = await _userManager.FindByIdAsync(userToUnfollowId);
            Guard.Against.NullItem(userToUnfollow, nameof(userToUnfollow));

            await _followerRepository.UnfollowUserAsync(userToUnfollow.Id, followerUser.Id);
        }
    }
}