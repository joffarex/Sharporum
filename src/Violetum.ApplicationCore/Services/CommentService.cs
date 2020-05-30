using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentValidators _commentValidators;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IPostValidators _postValidators;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;
        private readonly IVoteRepository _voteRepository;

        public CommentService(ICommentRepository commentRepository, UserManager<User> userManager,
            IPostRepository postRepository, IVoteRepository voteRepository, IMapper mapper,
            ICommentValidators commentValidators, IUserValidators userValidators, IPostValidators postValidators)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
            _postRepository = postRepository;
            _voteRepository = voteRepository;
            _mapper = mapper;
            _commentValidators = commentValidators;
            _userValidators = userValidators;
            _postValidators = postValidators;
        }

        public CommentViewModel GetComment(string commentId)
        {
            return _commentValidators.GetCommentByIdOrThrow(commentId, x => AttachVotesToCommentViewModel(x));
        }

        public async Task<IEnumerable<CommentViewModel>> GetComments(CommentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            return _commentRepository.GetComments(
                x => CommentHelpers.WhereConditionPredicate(searchParams.UserId, searchParams.PostId, x),
                x => AttachVotesToCommentViewModel(x),
                BaseHelpers.GetOrderByExpression<CommentViewModel>(searchParams.SortBy),
                searchParams
            );
        }

        public async Task<int> GetTotalCommentsCount(CommentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.PostId))
            {
                _postValidators.GetPostByIdOrThrow(searchParams.PostId, x => x);
            }

            return _commentRepository.GetTotalCommentsCount(
                x => CommentHelpers.WhereConditionPredicate(searchParams.UserId, searchParams.PostId, x)
            );
        }

        public async Task<CommentViewModel> CreateComment(CommentDto commentDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(commentDto.AuthorId);
            Post post = _postValidators.GetPostByIdOrThrow(commentDto.PostId, x => x);

            if (!string.IsNullOrEmpty(commentDto.ParentId))
            {
                Comment parentComment = _commentValidators.GetCommentByIdOrThrow(commentDto.ParentId, x => x);
                if (parentComment.Post.Id != post.Id)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "This operation is not allowed");
                }
            }

            var comment = _mapper.Map<Comment>(commentDto);
            comment.Author = user;
            comment.Post = post;

            await _commentRepository.CreateComment(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> UpdateComment(string commentId, string userId,
            UpdateCommentDto updateCommentDto)
        {
            Comment comment = _commentValidators.GetCommentByIdOrThrow(commentId, x => x);
            if (comment.PostId != updateCommentDto.PostId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                    $"{nameof(Comment)}:(cid[{comment.PostId}]|dtoid[{updateCommentDto.PostId}] update");
            }

            if (comment.AuthorId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            comment.Content = updateCommentDto.Content;

            await _commentRepository.UpdateComment(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<bool> DeleteComment(string commentId, string userId)
        {
            Comment comment = _commentValidators.GetCommentByIdOrThrow(commentId, x => x);
            if (comment.AuthorId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            return await _commentRepository.DeleteComment(comment) > 0;
        }

        public async Task VoteComment(string commentId, string userId, CommentVoteDto commentVoteDto)
        {
            try
            {
                User user = await _userValidators.GetUserByIdOrThrow(userId);
                Comment comment = _commentValidators.GetCommentByIdOrThrow(commentId, x => x);

                var commentVote =
                    _voteRepository.GetEntityVote<CommentVote>(
                        x => (x.CommentId == comment.Id) && (x.UserId == user.Id), x => x);

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

                    await _voteRepository.UpdateEntityVote(commentVote);
                }
                else
                {
                    var newCommentVote = new CommentVote
                    {
                        CommentId = comment.Id,
                        UserId = user.Id,
                        Direction = commentVoteDto.Direction,
                    };

                    newCommentVote.User = user;
                    newCommentVote.Comment = comment;

                    await _voteRepository.VoteEntity(newCommentVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }

        private CommentViewModel AttachVotesToCommentViewModel(Comment x)
        {
            var commentViewModel = _mapper.Map<CommentViewModel>(x);
            commentViewModel.VoteCount = _commentRepository.GetCommentVoteSum(x.Id);
            return commentViewModel;
        }
    }
}