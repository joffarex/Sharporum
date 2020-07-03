using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
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
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;
        private readonly IVoteRepository _voteRepository;

        public CommentService(ICommentRepository commentRepository, IVoteRepository voteRepository,
            IPostRepository postRepository, UserManager<User> userManager, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _voteRepository = voteRepository;
            _postRepository = postRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public CommentViewModel GetComment(string commentId)
        {
            var comment = _commentRepository.GetComment<CommentViewModel>(x => x.Id == commentId,
                CommentHelpers.GetCommentMapperConfiguration());
            Guard.Against.NullItem(comment, nameof(comment));

            return comment;
        }

        public Comment GetCommentEntity(string commentId)
        {
            Comment comment = _commentRepository.GetComment(x => x.Id == commentId);
            Guard.Against.NullItem(comment, nameof(comment));

            return comment;
        }

        public async Task<IEnumerable<CommentViewModel>> GetCommentsAsync(CommentSearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user.Id, nameof(user));

            return _commentRepository.GetComments<CommentViewModel>(searchParams,
                CommentHelpers.GetCommentMapperConfiguration());
        }

        public async Task<int> GetCommentsCountAsync(CommentSearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user, nameof(user));

            Post post = _postRepository.GetPost(x => x.Id == searchParams.PostId);
            Guard.Against.NullItem(post, nameof(post));

            return _commentRepository.GetCommentsCount(searchParams);
        }

        public async Task<string> CreateCommentAsync(string userId, CreateCommentDto createCommentDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            Post post = _postRepository.GetPost(x => x.Id == createCommentDto.PostId);
            Guard.Against.NullItem(post, nameof(post));

            if (!string.IsNullOrEmpty(createCommentDto.ParentId))
            {
                Comment parentComment = _commentRepository.GetComment(x => x.Id == createCommentDto.ParentId);

                Guard.Against.NotEqual(parentComment.Post.Id, post.Id);
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
                User user = await _userManager.FindByIdAsync(userId);
                Guard.Against.NullItem(user, nameof(user));

                Comment comment = _commentRepository.GetComment(x => x.Id == commentId);
                Guard.Against.NullItem(comment, nameof(comment));

                var commentVote = _voteRepository.GetEntityVote<CommentVote>(
                    x => (x.CommentId == comment.Id) && (x.UserId == user.Id),
                    x => x
                );

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