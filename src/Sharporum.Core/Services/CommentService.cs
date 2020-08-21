using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Sharporum.Core.Dtos.Comment;
using Sharporum.Core.Helpers;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Specifications;
using Sharporum.Core.ViewModels.Comment;
using Sharporum.Domain.CustomExceptions;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Infrastructure;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IAsyncRepository<Comment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<Post> _postRepository;
        private readonly UserManager<User> _userManager;
        private readonly IVoteRepository _voteRepository;

        public CommentService(IAsyncRepository<Comment> commentRepository, IVoteRepository voteRepository,
            IAsyncRepository<Post> postRepository, UserManager<User> userManager, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _voteRepository = voteRepository;
            _postRepository = postRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CommentViewModel> GetCommentByIdAsync(string commentId)
        {
            var comment = await _commentRepository.GetByConditionAsync<CommentViewModel>(x => x.Id == commentId,
                CommentHelpers.GetCommentMapperConfiguration());
            Guard.Against.NullItem(comment, nameof(comment));

            return comment;
        }

        public async Task<Comment> GetCommentEntityAsync(string commentId)
        {
            Comment comment = await _commentRepository.GetByConditionAsync(x => x.Id == commentId);
            Guard.Against.NullItem(comment, nameof(comment));

            return comment;
        }

        public async Task<IEnumerable<CommentViewModel>> GetCommentsAsync(CommentSearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user.Id, nameof(user));

            var specification = new CommentFilterSpecification(searchParams);

            return await _commentRepository.ListAsync<CommentViewModel>(specification,
                CommentHelpers.GetCommentMapperConfiguration());
        }

        public async Task<int> GetCommentsCountAsync(CommentSearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user, nameof(user));

            Post post = await _postRepository.GetByConditionAsync(x => x.Id == searchParams.PostId);
            Guard.Against.NullItem(post, nameof(post));

            var specification = new CommentFilterSpecification(searchParams);

            return await _commentRepository.GetTotalCountAsync(specification);
        }

        public async Task<string> CreateCommentAsync(string userId, CreateCommentDto createCommentDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            Post post = await _postRepository.GetByConditionAsync(x => x.Id == createCommentDto.PostId);
            Guard.Against.NullItem(post, nameof(post));

            if (!string.IsNullOrEmpty(createCommentDto.ParentId))
            {
                Comment parentComment =
                    await _commentRepository.GetByConditionAsync(x => x.Id == createCommentDto.ParentId);

                Guard.Against.NotEqual(parentComment.Post.Id, post.Id);
            }

            var comment = _mapper.Map<Comment>(createCommentDto);
            comment.Author = user;
            comment.Post = post;

            await _commentRepository.CreateAsync(comment);

            return comment.Id;
        }

        public async Task<CommentViewModel> UpdateCommentAsync(Comment comment, UpdateCommentDto updateCommentDto)
        {
            comment.Content = updateCommentDto.Content;

            await _commentRepository.UpdateAsync(comment);

            return _mapper.Map<CommentViewModel>(comment);
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            await _commentRepository.DeleteAsync(comment);
        }

        public async Task VoteCommentAsync(string commentId, string userId, CommentVoteDto commentVoteDto)
        {
            try
            {
                User user = await _userManager.FindByIdAsync(userId);
                Guard.Against.NullItem(user, nameof(user));

                Comment comment = await _commentRepository.GetByConditionAsync(x => x.Id == commentId);
                Guard.Against.NullItem(comment, nameof(comment));

                var commentVote = await _voteRepository.GetEntityVoteAsync<CommentVote>(
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