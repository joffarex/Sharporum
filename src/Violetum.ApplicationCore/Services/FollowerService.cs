using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
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

        public FollowerService(IFollowerRepository followerRepository, UserManager<User> userManager, IMapper mapper)
        {
            _followerRepository = followerRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserFollowersViewModel> GetUserFollowers(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(User)}:{userId} not found");
            }

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
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(User)}:{userId} not found");
            }

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
            await ValidateFollowerDto(followerDto);

            var follower = new Follower
            {
                UserToFollowId = followerDto.UserToFollowId,
                FollowerUserId = followerDto.FollowerUserId,
            };

            await _followerRepository.FollowUser(follower);
        }

        public async Task UnfollowUser(FollowerDto followerDto)
        {
            await ValidateFollowerDto(followerDto);

            await _followerRepository.UnfollowUser(followerDto.UserToFollowId, followerDto.FollowerUserId);
        }

        private async Task ValidateFollowerDto(FollowerDto followerDto)
        {
            if (followerDto == null)
            {
                throw new ArgumentNullException(nameof(followerDto));
            }

            User userToFollow = await _userManager.FindByIdAsync(followerDto.UserToFollowId);
            if (userToFollow == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{followerDto.UserToFollowId} not found");
            }

            User followerUser = await _userManager.FindByIdAsync(followerDto.FollowerUserId);
            if (followerUser == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{followerDto.FollowerUserId} not found");
            }
        }
    }
}