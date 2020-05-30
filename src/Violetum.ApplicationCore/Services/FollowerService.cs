using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Follower;
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
                _followerRepository.GetUserFollowers(userId, x => _mapper.Map<FollowerViewModel>(x));

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
                _followerRepository.GetUserFollowing(userId, x => _mapper.Map<FollowingViewModel>(x));

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

        public async Task FollowUser(FollowerDto followerDto)
        {
            User userToFollow = await _userValidators.GetUserByIdOrThrow(followerDto.UserToFollowId);
            User followerUser = await _userValidators.GetUserByIdOrThrow(followerDto.FollowerUserId);

            var follower = new Follower
            {
                UserToFollowId = userToFollow.Id,
                FollowerUserId = followerUser.Id,
            };

            await _followerRepository.FollowUser(follower);
        }

        public async Task UnfollowUser(FollowerDto followerDto)
        {
            User userToFollow = await _userValidators.GetUserByIdOrThrow(followerDto.UserToFollowId);
            User followerUser = await _userValidators.GetUserByIdOrThrow(followerDto.FollowerUserId);

            await _followerRepository.UnfollowUser(userToFollow.Id, followerUser.Id);
        }
    }
}