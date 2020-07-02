using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Commands.Post;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Queries.Post;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly HttpContext _httpContext;
        private readonly IMediator _mediator;

        public PostsController(IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService, IMediator mediator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _authorizationService = authorizationService;
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns posts
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns posts</response>
        /// <response code="404">
        ///     Unable to find user with provided "UserId" / community with provided "CommunityName" / post with
        ///     provided "PostTitle
        /// </response>
        [HttpGet(ApiRoutes.Posts.GetMany)]
        [Cached(60)]
        [ProducesResponseType(typeof(GetManyResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] PostSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            var query = new GetPostsQuery(searchParams);
            GetManyResponse<PostViewModel> result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        ///     Creates post
        /// </summary>
        /// <param name="createPostDto"></param>
        /// <response code="200">Creates post</response>
        /// <response code="400">Unable to create post due to validation errors</response>
        /// <response code="404">Unable to find user with provided "AuthorId"/ community with provided "CommunityId"</response>
        [HttpPost(ApiRoutes.Posts.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(CreatedResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new CreatePostCommand(userId, createPostDto);
            CreatedResponse result = await _mediator.Send(command);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{result.Id}", result);
        }

        /// <summary>
        ///     Returns newsfeed
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns newsfeed</response>
        /// <response code="404">
        ///     Unable to find user with provided "UserId" / community with provided "CommunityName" / post with
        ///     provided "PostTitle
        /// </response>
        [HttpGet(ApiRoutes.Posts.NewsFeed)]
        [Cached(180)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(GetManyResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> NewsFeed([FromQuery] PostSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            string userId = _httpContext.User.FindFirstValue("sub");

            var query = new GetNewsFeedQuery(userId, searchParams);
            GetManyResponse<PostViewModel> result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        ///     Returns post
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="200">Returns post</response>
        /// <response code="404">Unable to find post with provided "postId"</response>
        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(60)]
        [ProducesResponseType(typeof(PostResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string postId)
        {
            var query = new GetPostQuery(postId);
            PostResponse result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        ///     Updates post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="updatePostDto"></param>
        /// <response code="200">Updates post</response>
        /// <response code="400">Unable to update post due to validation errors</response>
        /// <response code="404">Unable to find post with provided "postId"</response>
        [HttpPut(ApiRoutes.Posts.Update)]
        [ProducesResponseType(typeof(PostResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromRoute] string postId, [FromBody] UpdatePostDto updatePostDto)
        {
            var query = new GetPostEntityQuery(postId);
            Post post = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, post, PolicyConstants.UpdatePostRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new UpdatePostCommand(post, updatePostDto);
            PostResponse result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        ///     Deletes post
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="200">Deletes post</response>
        [HttpDelete(ApiRoutes.Posts.Delete)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Delete([FromRoute] string postId)
        {
            var query = new GetPostEntityQuery(postId);
            Post post = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, post, PolicyConstants.DeletePostRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new DeletePostCommand(post);
            await _mediator.Send(command);

            return Ok();
        }

        /// <summary>
        ///     Votes post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="postVoteDto"></param>
        /// <response code="200">Votes post</response>
        /// <response code="422">Unable to vote post due to validation errors</response>
        [HttpPost(ApiRoutes.Posts.Vote)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Vote([FromRoute] string postId, [FromBody] PostVoteDto postVoteDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new VotePostCommand(userId, postId, postVoteDto);
            await _mediator.Send(command);

            return Ok();
        }
    }
}