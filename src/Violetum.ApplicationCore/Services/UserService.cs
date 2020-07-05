using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Specifications;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IAsyncRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<Post> _postRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<User> userManager, IAsyncRepository<Post> postRepository,
            IAsyncRepository<Comment> commentRepository, IUserRepository userRepository, IMapper mapper)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserViewModel> GetUserAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewModel> UpdateUserAsync(string userId,
            UpdateUserDto updateUserDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.UserName;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Gender = updateUserDto.Gender;
            user.BirthDate = updateUserDto.Birthdate;

            await _userManager.UpdateAsync(user);

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewModel> UpdateUserImageAsync(string userId,
            UpdateUserImageDto updateUserImageDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            user.Image = updateUserImageDto.Image;

            await _userManager.UpdateAsync(user);

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<IReadOnlyList<UserRank>> GetUserRanksAsync(string userId)
        {
            var postFilterSpecification = new PostFilterSpecification(new PostSearchParams {UserId = userId});
            int userPostsCount = await _postRepository.GetTotalCountAsync(postFilterSpecification);
            double userPostRank = await _userRepository.GetUserPostRank(userId) / userPostsCount;

            var commentFilterSpecification = new CommentFilterSpecification(new CommentSearchParams {UserId = userId});
            int userCommentsCount = await _commentRepository.GetTotalCountAsync(commentFilterSpecification);
            double userCommentRank = await _userRepository.GetUserCommentRank(userId) / userCommentsCount;

            var userRanks = new List<UserRank>
            {
                new UserRank {Entity = nameof(Post), Rank = userPostRank},
                new UserRank {Entity = nameof(Comment), Rank = userCommentRank},
            };

            return userRanks;
        }

        public async Task<IReadOnlyList<Ranks>> GetPostRanksAsync()
        {
            IReadOnlyList<Ranks> userRanks = await _userRepository.ListRanks<Post>();

            foreach (Ranks userRank in userRanks)
            {
                var specification = new PostFilterSpecification(new PostSearchParams {UserId = userRank.UserId});
                int userPostsCount = await _postRepository.GetTotalCountAsync(specification);
                userRank.Rank /= userPostsCount;
            }

            return userRanks;
        }

        public async Task<IReadOnlyList<Ranks>> GetCommentRanksAsync()
        {
            IReadOnlyList<Ranks> userRanks = await _userRepository.ListRanks<Comment>();

            foreach (Ranks userRank in userRanks)
            {
                var specification = new CommentFilterSpecification(new CommentSearchParams {UserId = userRank.UserId});
                int userCommentsCount = await _commentRepository.GetTotalCountAsync(specification);
                userRank.Rank /= userCommentsCount;
            }

            return userRanks;
        }

        public async Task<UserFollowersViewModel> GetUserFollowersAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            IEnumerable<FollowerViewModel> userFollowers =
                await _userRepository.ListUserFollowersAsync<FollowerViewModel>(userId,
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

        public async Task<bool> IsAuthenticatedUserFollowerAsync(string userToFollowId, string authenticatedUserId)
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