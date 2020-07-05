using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
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

        public async Task<IReadOnlyList<UserRank>> GetUserRanks(string userId)
        {
            int userPostsCount = await _postRepository.GetTotalCountAsync(new PostSearchParams
            {
                UserId = userId,
            });
            double userPostRank = await _userRepository.GetUserPostRank(userId) / userPostsCount;

            int userCommentsCount = await _commentRepository.GetTotalCountAsync(new CommentSearchParams
            {
                UserId = userId,
            });
            double userCommentRank = await _userRepository.GetUserCommentRank(userId) / userCommentsCount;

            var userRanks = new List<UserRank>
            {
                new UserRank {Entity = nameof(Post), Rank = userPostRank},
                new UserRank {Entity = nameof(Comment), Rank = userCommentRank},
            };

            return userRanks;
        }

        public async Task<IReadOnlyList<Ranks>> GetPostRanks()
        {
            IReadOnlyList<Ranks> userRanks = await _userRepository.ListRanks<Post>();

            foreach (Ranks userRank in userRanks)
            {
                int userPostsCount = await _postRepository.GetTotalCountAsync(new PostSearchParams
                {
                    UserId = userRank.UserId,
                });
                userRank.Rank /= userPostsCount;
            }

            return userRanks;
        }

        public async Task<IReadOnlyList<Ranks>> GetCommentRanks()
        {
            IReadOnlyList<Ranks> userRanks = await _userRepository.ListRanks<Comment>();

            foreach (Ranks userRank in userRanks)
            {
                int userCommentsCount = await _commentRepository.GetTotalCountAsync(new CommentSearchParams
                {
                    UserId = userRank.UserId,
                });
                userRank.Rank /= userCommentsCount;
            }

            return userRanks;
        }
    }
}