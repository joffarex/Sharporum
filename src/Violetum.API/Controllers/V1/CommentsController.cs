using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Contracts.V1;
using Violetum.API.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class CommentsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ICommentService _commentService;
        private readonly IIdentityManager _identityManager;

        public CommentsController(ICommentService commentService, IIdentityManager identityManager,
            IAuthorizationService authorizationService)
        {
            _commentService = commentService;
            _identityManager = identityManager;
            _authorizationService = authorizationService;
        }

        [HttpGet(ApiRoutes.Comments.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] CommentSearchParams searchParams)
        {
            IEnumerable<CommentViewModel> comments = await _commentService.GetComments(searchParams);
            int commentsCount = await _commentService.GetTotalCommentsCount(searchParams);

            return Ok(new GetManyResponse<CommentViewModel>
            {
                Data = comments,
                Count = commentsCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
            });
        }

        [HttpPost(ApiRoutes.Comments.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            string userId = _identityManager.GetUserId();
            createCommentDto.AuthorId = userId;

            CommentViewModel comment = await _commentService.CreateComment(createCommentDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new CommentResponse {Comment = comment});
        }

        [HttpGet(ApiRoutes.Comments.Get)]
        public IActionResult Get(string commentId)
        {
            return Ok(new CommentResponse {Comment = _commentService.GetComment(commentId)});
        }

        [HttpPut(ApiRoutes.Comments.Update)]
        public async Task<IActionResult> Update([FromRoute] string commentId,
            [FromBody] UpdateCommentDto updateCommentDto)
        {
            CommentViewModel commentViewModel = _commentService.GetComment(commentId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, commentViewModel,
                    PolicyConstants.UpdateCommentRolePolicy);
            if (authorizationResult.Succeeded)
            {
                CommentViewModel comment = await _commentService.UpdateComment(commentViewModel, updateCommentDto);

                return Ok(new CommentResponse {Comment = comment});
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }

        [HttpDelete(ApiRoutes.Comments.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string commentId)
        {
            CommentViewModel comment = _commentService.GetComment(commentId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, comment,
                    PolicyConstants.UpdateCommentRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _commentService.DeleteComment(comment);

                return Ok(new ActionSuccessResponse {Message = "OK"});
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }

        [HttpPost(ApiRoutes.Comments.Vote)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Vote([FromRoute] string commentId, [FromBody] CommentVoteDto commentVoteDto)
        {
            string userId = _identityManager.GetUserId();

            await _commentService.VoteComment(commentId, userId, commentVoteDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}