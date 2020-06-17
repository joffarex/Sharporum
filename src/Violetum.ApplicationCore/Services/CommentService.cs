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
            return _commentValidators.GetCommentOrThrow<CommentViewModel>(x => x.Id == commentId);
        }

        public Comment GetCommentEntity(string commentId)
        {
            return _commentValidators.GetCommentOrThrow(x => x.Id == commentId);
        }

        public async Task<IEnumerable<CommentViewModel>> GetCommentsAsync(CommentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            return _commentRepository.GetComments<CommentViewModel>(searchParams,
                CommentHelpers.GetCommentMapperConfiguration());
        }

        public async Task<int> GetCommentsCountAsync(CommentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.PostId))
            {
                _postValidators.GetPostOrThrow(x => x.Id == searchParams.PostId);
            }

            return _commentRepository.GetCommentsCount(searchParams);
        }

        public async Task<string> CreateCommentAsync(string userId, CreateCommentDto createCommentDto)
        {
            User user = await _userValidators.GetUserByIdOrThrowAsync(userId);
            Post post = _postValidators.GetPostOrThrow(x => x.Id == createCommentDto.PostId);

            if (!string.IsNullOrEmpty(createCommentDto.ParentId))
            {
                Comment parentComment =
                    _commentValidators.GetCommentOrThrow(x => x.Id == createCommentDto.ParentId);
                if (parentComment.Post.Id != post.Id)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "This operation is not allowed");
                }
            }

            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.Author = user;
            comment.Post = post;

            await _commentRepository.CreateCommentAsync(comment);

            return comment.Id;
        }

        public async Task<CommentViewModel> UpdateCommentAsync(Comment comment, UpdateCommentDto updateCommentDto)
        {
            comment.Content = updateCommentDto.Content;

            await _commentRepository.UpdateCommentAsync(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            await _commentRepository.DeleteCommentAsync(comment);
        }

        public async Task VoteCommentAsync(string commentId, string userId, CommentVoteDto commentVoteDto)
        {
            try
            {
                User user = await _userValidators.GetUserByIdOrThrowAsync(userId);
                Comment comment = _commentValidators.GetCommentOrThrow(x => x.Id == commentId);

                var commentVote =
                    _voteRepository.GetEntityVote<CommentVote>(
                        x => (x.CommentId == comment.Id) && (x.UserId == user.Id), x => x);

                if (commentVote != null)
                {
                    commentVote.Direction =
                        commentVote.Direction == commentVoteDto.Direction ? 0 : commentVoteDto.Direction;

                    await _voteRepository.UpdateEntityVoteAsync(commentVote);
                }
                else
                {
                    var newCommentVote = new CommentVote
                    {
                        CommentId = comment.Id,
                        UserId = user.Id,
                        Direction = commentVoteDto.Direction,
                    };

                    await _voteRepository.VoteEntityAsync(newCommentVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }
    }
}