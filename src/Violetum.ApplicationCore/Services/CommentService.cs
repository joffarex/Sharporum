using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.ApplicationCore.Attributes;
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
        private readonly IPostValidators _postValidators;
        private readonly IUserValidators _userValidators;
        private readonly IVoteRepository _voteRepository;

        public CommentService(ICommentRepository commentRepository, IVoteRepository voteRepository, IMapper mapper,
            ICommentValidators commentValidators, IUserValidators userValidators, IPostValidators postValidators)
        {
            _commentRepository = commentRepository;
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

        public async Task<CommentViewModel> CreateComment(string userId, CreateCommentDto createCommentDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);
            Post post = _postValidators.GetPostByIdOrThrow(createCommentDto.PostId, x => x);

            if (!string.IsNullOrEmpty(createCommentDto.ParentId))
            {
                Comment parentComment = _commentValidators.GetCommentByIdOrThrow(createCommentDto.ParentId, x => x);
                if (parentComment.Post.Id != post.Id)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "This operation is not allowed");
                }
            }

            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.Author = user;
            comment.Post = post;

            await _commentRepository.CreateComment(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> UpdateComment(CommentViewModel commentViewModel,
            UpdateCommentDto updateCommentDto)
        {
            Comment comment = _commentValidators.GetCommentByIdOrThrow(commentViewModel.Id, x => x);
            comment.Content = updateCommentDto.Content;

            await _commentRepository.UpdateComment(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task<bool> DeleteComment(CommentViewModel commentViewModel)
        {
            return await _commentRepository.DeleteComment(_mapper.Map<Comment>(commentViewModel)) > 0;
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