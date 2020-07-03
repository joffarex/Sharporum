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
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager, IPostRepository postRepository,
            ICommentRepository commentRepository, IMapper mapper)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
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

        public IEnumerable<UserRank> GetUserRanks(string userId)
        {
            int userPostsCount = _postRepository.GetPostCount(new PostSearchParams
            {
                UserId = userId,
            });
            double userPostRank = _postRepository.GetUserPostRank(userId) / (double) userPostsCount;

            int userCommentsCount = _commentRepository.GetCommentsCount(new CommentSearchParams
            {
                UserId = userId,
            });
            double userCommentRank = _commentRepository.GetUserCommentRank(userId) / (double) userCommentsCount;

            var userRanks = new List<UserRank>
            {
                new UserRank {Entity = nameof(Post), Rank = userPostRank},
                new UserRank {Entity = nameof(Comment), Rank = userCommentRank},
            };

            return userRanks;
        }

        public IEnumerable<Ranks> GetPostRanks()
        {
            List<Ranks> userRanks = _postRepository.GetPostRanks();

            foreach (Ranks userRank in userRanks)
            {
                int userPostsCount = _postRepository.GetPostCount(new PostSearchParams
                {
                    UserId = userRank.UserId,
                });
                userRank.Rank /= userPostsCount;
            }

            return userRanks;
        }

        public IEnumerable<Ranks> GetCommentRanks()
        {
            List<Ranks> userRanks = _commentRepository.GetCommentRanks();

            foreach (Ranks userRank in userRanks)
            {
                int userCommentsCount = _commentRepository.GetCommentsCount(new CommentSearchParams
                {
                    UserId = userRank.UserId,
                });
                userRank.Rank /= userCommentsCount;
            }

            return userRanks;
        }
    }
}