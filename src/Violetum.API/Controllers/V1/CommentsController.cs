using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ITokenManager _tokenManager;

        public CommentsController(ICommentService commentService, ITokenManager tokenManager)
        {
            _commentService = commentService;
            _tokenManager = tokenManager;
        }

        [HttpGet(ApiRoutes.Comments.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] CommentSearchParams searchParams)
        {
            IEnumerable<CommentViewModel> comments = await _commentService.GetComments(searchParams);
            int commentsCount = await _commentService.GetTotalCommentsCount(searchParams);

            return Ok(new
            {
                Comments = comments, CommentsCount = commentsCount,
                Params = new {searchParams.Limit, searchParams.CurrentPage},
            });
        }

        [HttpPost(ApiRoutes.Comments.Create)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            createCommentDto.AuthorId = userId;
            
            CommentViewModel comment = await _commentService.CreateComment(createCommentDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new {Comment = comment});
        }

        [HttpGet(ApiRoutes.Comments.Get)]
        public IActionResult Get(string commentId)
        {
            return Ok(new {Comment = _commentService.GetComment(commentId)});
        }

        [HttpPut(ApiRoutes.Comments.Update)]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] string commentId,
            [FromBody] UpdateCommentDto updateCommentDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            CommentViewModel comment = await _commentService.UpdateComment(commentId, userId, updateCommentDto);

            return Ok(new {Comment = comment});
        }

        [HttpDelete(ApiRoutes.Comments.Delete)]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string commentId)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _commentService.DeleteComment(commentId, userId);

            return Ok(new {Message = "OK"});
        }

        [HttpPost(ApiRoutes.Comments.Vote)]
        [Authorize]
        public async Task<IActionResult> Vote([FromRoute] string commentId, [FromBody] CommentVoteDto commentVoteDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _commentService.VoteComment(commentId, userId, commentVoteDto);

            return Ok(new {Message = "OK"});
        }
    }
}