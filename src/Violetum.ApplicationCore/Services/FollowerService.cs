using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class FollowerService : IFollowerService
    {
        private readonly IFollowerRepository _followerRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;

        public FollowerService(IFollowerRepository followerRepository, UserManager<User> userManager, IMapper mapper,
            IUserValidators userValidators)
        {
            _followerRepository = followerRepository;
            _userManager = userManager;
            _mapper = mapper;
            _userValidators = userValidators;
        }

        public async Task<UserFollowersViewModel> GetUserFollowers(string userId)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);

            IEnumerable<FollowerViewModel> userFollowers =
                _followerRepository.GetUserFollowing<FollowerViewModel>(userId,
                    UserHelpers.GetFollowerMapperConfiguration());

            return new UserFollowersViewModel
            {
                UserId = user.Id,
                Followers = userFollowers,
            };
        }

        public async Task<UserFollowingViewModel> GetUserFollowing(string userId)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);

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

        public async Task FollowUser(string userId, string userToFollowId)
        {
            User followerUser = await _userValidators.GetUserByIdOrThrow(userId);
            User userToFollow = await _userValidators.GetUserByIdOrThrow(userToFollowId);

            var follower = new Follower
            {
                UserToFollowId = userToFollow.Id,
                FollowerUserId = followerUser.Id,
            };

            await _followerRepository.FollowUser(follower);
        }

        public async Task UnfollowUser(string userId, string userToUnfollowId)
        {
            User followerUser = await _userValidators.GetUserByIdOrThrow(userId);
            User userToUnfollow = await _userValidators.GetUserByIdOrThrow(userToUnfollowId);

            await _followerRepository.UnfollowUser(userToUnfollow.Id, followerUser.Id);
        }
    }
}