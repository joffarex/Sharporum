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
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public FollowerService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<UserFollowersViewModel> GetUserFollowersAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            IEnumerable<FollowerViewModel> userFollowers =
                await _userRepository.ListUserFollowingAsync<FollowerViewModel>(userId,
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
                await _userRepository.ListUserFollowingAsync<FollowingViewModel>(userId,
                    UserHelpers.GetFollowerMapperConfiguration());

            return new UserFollowingViewModel
            {
                UserId = user.Id,
                Followings = userFollowings,
            };
        }

        public async Task<bool> IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId)
        {
            return await _userRepository.IsAuthenticatedUserFollower(userToFollowId, authenticatedUserId);
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

            await _userRepository.FollowUserAsync(follower);
        }

        public async Task UnfollowUserAsync(string userId, string userToUnfollowId)
        {
            User followerUser = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(followerUser, nameof(followerUser));

            User userToUnfollow = await _userManager.FindByIdAsync(userToUnfollowId);
            Guard.Against.NullItem(userToUnfollow, nameof(userToUnfollow));

            await _userRepository.UnfollowUserAsync(userToUnfollow.Id, followerUser.Id);
        }
    }
}