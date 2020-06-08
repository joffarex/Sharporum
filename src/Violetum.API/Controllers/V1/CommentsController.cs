using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
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

        /// <summary>
        ///     Returns comments
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns comments</response>
        /// <response code="404">Unable to find user with provided "UserId" / post with provided "PostId"</response>
        [HttpGet(ApiRoutes.Comments.GetMany)]
        [ProducesResponseType(typeof(GetManyResponse<CommentViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
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

        /// <summary>
        ///     Creates comment
        /// </summary>
        /// <param name="createCommentDto"></param>
        /// <response code="200">Creates comment</response>
        /// <response code="400">Unable to create comment due to validation errors</response>
        /// <response code="404">
        ///     Unable to find user with provided "AuthorId"/ post with provided "PostId" / comment with provided
        ///     "ParentId"
        /// </response>
        [HttpPost(ApiRoutes.Comments.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(CommentResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            string userId = _identityManager.GetUserId();

            CommentViewModel comment = await _commentService.CreateComment(userId, createCommentDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new CommentResponse {Comment = comment});
        }

        /// <summary>
        ///     Returns comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="200">Returns comment</response>
        /// <response code="404">Unable to find comment with provided "commentId"</response>
        [HttpGet(ApiRoutes.Comments.Get)]
        [ProducesResponseType(typeof(CommentResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public IActionResult Get([FromRoute] string commentId)
        {
            return Ok(new CommentResponse {Comment = _commentService.GetComment(commentId)});
        }

        /// <summary>
        ///     Updates comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="updateCommentDto"></param>
        /// <response code="200">Updates comment</response>
        /// <response code="400">Unable to update comment due to validation errors</response>
        /// <response code="404">Unable to find comment with provided "commentId"</response>
        [HttpPut(ApiRoutes.Comments.Update)]
        [ProducesResponseType(typeof(CommentResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
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

        /// <summary>
        ///     Deletes comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="200">Deletes comment</response>
        [HttpDelete(ApiRoutes.Comments.Delete)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.OK)]
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

        /// <summary>
        ///     Votes comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="commentVoteDto"></param>
        /// <response code="200">Votes comment</response>
        /// <response code="422">Unable to vote comment due to validation errors</response>
        [HttpPost(ApiRoutes.Comments.Vote)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Vote([FromRoute] string commentId, [FromBody] CommentVoteDto commentVoteDto)
        {
            string userId = _identityManager.GetUserId();

            await _commentService.VoteComment(commentId, userId, commentVoteDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}