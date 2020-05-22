using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
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

        public CommentViewModel GetComment(string commentId)
        {
            CommentViewModel comment =
                _commentRepository.GetCommentById(commentId, x => AttachVotesToCommentViewModel(x));
            if (comment == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Comment)}:{commentId} not found");
            }

            return comment;
        }

        public async Task<IEnumerable<CommentViewModel>> GetComments(SearchParams searchParams)
        {
            await ValidateSearchParams(searchParams);

            return _commentRepository.GetComments(
                x => Predicate(searchParams.UserId, searchParams.PostId, x),
                x => AttachVotesToCommentViewModel(x),
                GetOrderByExpression<CommentViewModel>(searchParams.SortBy),
                searchParams
            );
        }

        public async Task<int> GetTotalCommentsCount(SearchParams searchParams)
        {
            await ValidateSearchParams(searchParams);

            return _commentRepository.GetTotalCommentsCount(
                x => Predicate(searchParams.UserId, searchParams.PostId, x),
                GetOrderByExpression<CommentViewModel>(searchParams.SortBy)
            );
        }

        public async Task<CommentViewModel> CreateComment(CommentDto commentDto)
        {
            CommentDtoValidationData validatedData = await ValidateCommentDto(commentDto);

            var comment = _mapper.Map<Comment>(commentDto);
            comment.Author = validatedData.User;
            comment.Post = validatedData.Post;

            await _commentRepository.CreateComment(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> UpdateComment(string commentId, string userId,
            UpdateCommentDto updateCommentDto)
        {
            Comment comment = ValidateCommentActionData(commentId, userId, updateCommentDto.Id);

            comment.Content = updateCommentDto.Content;

            await _commentRepository.UpdateComment(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<bool> DeleteComment(string commentId, string userId, DeleteCommentDto deleteCommentDto)
        {
            Comment comment = ValidateCommentActionData(commentId, userId, deleteCommentDto.Id);

            return await _commentRepository.DeleteComment(comment) > 0;
        }

        public async Task VoteComment(string commentId, string userId, CommentVoteDto commentVoteDto)
        {
            try
            {
                CommentVoteDtoValidationData validatedData =
                    await ValidateCommentVoteActionData(commentId, userId, commentVoteDto);

                CommentVote commentVote = _commentRepository.GetCommentVote(commentId, userId, x => x);

                if (commentVote != null)
                {
                    if (commentVote.Direction == commentVoteDto.Direction)
                    {
                        commentVote.Direction = 0;
                    }
                    else
                    {
                        commentVote.Direction = commentVoteDto.Direction;
                    }

                    await _commentRepository.UpdateCommentVote(commentVote);
                }
                else
                {
                    var newCommentVote = new CommentVote
                    {
                        CommentId = commentVoteDto.CommentId,
                        UserId = commentVoteDto.UserId,
                        Direction = commentVoteDto.Direction,
                    };

                    newCommentVote.User = validatedData.User;
                    newCommentVote.Comment = validatedData.Comment;

                    await _commentRepository.VoteComment(newCommentVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }

        public int GetCommentVoteSum(string commentId)
        {
            Comment comment = _commentRepository.GetCommentById(commentId, x => x);
            if (comment == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Comment)}:{commentId} not found");
            }

            return _commentRepository.GetCommentVoteSum(comment.Id);
        }

        private CommentViewModel AttachVotesToCommentViewModel(Comment x)
        {
            var commentViewModel = _mapper.Map<CommentViewModel>(x);
            commentViewModel.VoteCount = GetCommentVoteSum(x.Id);
            return commentViewModel;
        }

        private async Task<CommentVoteDtoValidationData> ValidateCommentVoteActionData(string commentId, string userId,
            CommentVoteDto commentVoteDto)
        {
            if (commentVoteDto == null)
            {
                throw new ArgumentNullException(nameof(commentVoteDto));
            }

            User user = await _userManager.FindByIdAsync(commentVoteDto.UserId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{commentVoteDto.UserId} not found");
            }

            Comment comment = _commentRepository.GetCommentById(commentVoteDto.CommentId, x => x);
            if (comment == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Comment)}:{commentVoteDto.CommentId} not found");
            }

            return new CommentVoteDtoValidationData
            {
                User = user,
                Comment = comment,
            };
        }

        private async Task ValidateSearchParams(SearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                User user = await _userManager.FindByIdAsync(searchParams.UserId);
                if (user == null)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                        $"{nameof(User)}:{searchParams.UserId} not found");
                }
            }

            if (!string.IsNullOrEmpty(searchParams.PostId))
            {
                Post post = _postRepository.GetPostById(searchParams.PostId, x => x);
                if (post == null)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                        $"{nameof(Post)}:{searchParams.PostId} not found");
                }
            }
        }

        private static bool Predicate(string userId, string postId, Comment c)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return c.AuthorId == userId;
            }

            if (!string.IsNullOrEmpty(postId))
            {
                return c.PostId == postId;
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
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{commentDto.AuthorId} not found");
            }

            Post post = _postRepository.GetPostById(commentDto.PostId, x => x);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Post)}:{commentDto.PostId} not found");
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
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    $"{MethodBase.GetCurrentMethod().Name} failed");
            }

            Comment comment = _commentRepository.GetCommentById(commentId, x => x);
            if (comment == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Comment)}:{commentId} not found");
            }

            if (comment.AuthorId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            return comment;
        }

        private Func<T, object> GetOrderByExpression<T>(string sortColumn)
        {
            Func<T, object> orderByExpr = null;
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Type sponsorResultType = typeof(T);

                if (sponsorResultType.GetProperties().Any(prop => prop.Name == sortColumn))
                {
                    PropertyInfo pinfo = sponsorResultType.GetProperty(sortColumn);
                    orderByExpr = data => pinfo.GetValue(data, null);
                }
            }

            return orderByExpr;
        }
    }

    public class CommentDtoValidationData
    {
        public User User { get; set; }
        public Post Post { get; set; }
    }

    public class CommentVoteDtoValidationData
    {
        public User User { get; set; }
        public Comment Comment { get; set; }
    }
}