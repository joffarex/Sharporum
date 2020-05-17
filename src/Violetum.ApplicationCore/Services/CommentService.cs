using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;

        public CommentService(ICommentRepository commentRepository, UserManager<User> userManager,
            IPostRepository postRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentViewModel>> GetComments(SearchParams searchParams, Paginator paginator)
        {
            await ValidateSearchParams(searchParams);

            return _commentRepository.GetComments(x => Predicate(searchParams.UserId, x),
                x => _mapper.Map<CommentViewModel>(x), paginator);
        }

        public async Task<CommentViewModel> CreateComment(CommentDto commentDto)
        {
            CommentDtoValidationData validatedData = await ValidateCommentDto(commentDto);

            var comment = _mapper.Map<Comment>(commentDto);
            comment.Author = validatedData.User;
            comment.Post = validatedData.Post;

            bool result = await _commentRepository.CreateComment(comment) > 0;

            if (!result)
            {
                throw new Exception("Create comment failed");
            }

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> UpdateComment(string commentId, string userId,
            UpdateCommentDto updateCommentDto)
        {
            Comment comment = ValidateCommentActionData(commentId, userId, updateCommentDto.Id);

            comment.Content = updateCommentDto.Content;

            bool result = await _commentRepository.UpdateComment(comment) > 0;

            if (!result)
            {
                throw new Exception("update post failed");
            }

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<bool> DeleteComment(string commentId, string userId, DeleteCommentDto deleteCommentDto)
        {
            Comment comment = ValidateCommentActionData(commentId, userId, deleteCommentDto.Id);

            return await _commentRepository.DeleteComment(comment) > 0;
        }

        private async Task ValidateSearchParams(SearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
        }

        private static bool Predicate(string userId, Comment c)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return c.AuthorId == userId;
            }

            return true;
        }

        private async Task<CommentDtoValidationData> ValidateCommentDto(CommentDto commentDto)
        {
            if (commentDto == null)
            {
                throw new ArgumentNullException(nameof(commentDto));
            }

            User user = await _userManager.FindByIdAsync(commentDto.AuthorId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            Post post = _postRepository.GetPostById(commentDto.PostId, x => x);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            return new CommentDtoValidationData
            {
                User = user,
                Post = post,
            };
        }

        private Comment ValidateCommentActionData(string commentId, string userId, string dtoCommentId)
        {
            if (commentId != dtoCommentId)
            {
                throw new Exception("Comment data validation failed");
            }

            Comment comment = _commentRepository.GetCommentById(commentId, x => x);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            if (comment.AuthorId != userId)
            {
                throw new Exception("Unauthorized user");
            }

            return comment;
        }
    }

    public class CommentDtoValidationData
    {
        public User User { get; set; }
        public Post Post { get; set; }
    }
}