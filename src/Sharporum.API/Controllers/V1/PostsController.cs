using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sharporum.API.Authorization;
using Sharporum.API.Contracts.V1;
using Sharporum.API.Filters;
using Sharporum.API.Helpers;
using Sharporum.Core.Commands.Post;
using Sharporum.Core.Dtos.Post;
using Sharporum.Core.Helpers;
using Sharporum.Core.Queries.Post;
using Sharporum.Core.Responses;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.API.Controllers.V1
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
        [ProducesResponseType(typeof(FilteredResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] PostSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out ErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            FilteredDataViewModel<PostViewModel> result = await _mediator.Send(new GetPostsQuery(searchParams));

            return Ok(new FilteredResponse<PostViewModel>(searchParams)
            {
                Data = result.Data,
                Count = result.Count,
            });
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
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new CreatePostCommand(userId, createPostDto);
            string postId = await _mediator.Send(command);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{postId}", null);
        }

        /// <summary>
        ///     Creates post with file
        /// </summary>
        /// <param name="createPostWithFileDto"></param>
        /// <response code="200">Creates post with file</response>
        /// <response code="400">Unable to create post due to validation errors</response>
        /// <response code="404">Unable to find user with provided "AuthorId"/ community with provided "CommunityId"</response>
        [HttpPost(ApiRoutes.Posts.CreateWithFile)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateWithFile([FromBody] CreatePostWithFileDto createPostWithFileDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new CreatePostWithFileCommand(userId, createPostWithFileDto);
            string postId = await _mediator.Send(command);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{postId}", null);
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
        [ProducesResponseType(typeof(FilteredResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> NewsFeed([FromQuery] PostSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out ErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            string userId = _httpContext.User.FindFirstValue("sub");

            FilteredDataViewModel<PostViewModel> result =
                await _mediator.Send(new GetNewsFeedQuery(userId, searchParams));

            return Ok(new FilteredResponse<PostViewModel>(searchParams)
            {
                Data = result.Data,
                Count = result.Count,
            });
        }

        /// <summary>
        ///     Returns post
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="200">Returns post</response>
        /// <response code="404">Unable to find post with provided "postId"</response>
        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(60)]
        [ProducesResponseType(typeof(Response<PostViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string postId)
        {
            return Ok(new Response<PostViewModel>
            {
                Data = await _mediator.Send(new GetPostQuery(postId)),
            });
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
        [ProducesResponseType(typeof(Response<PostViewModel>), (int) HttpStatusCode.Created)]
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

            return Ok(new Response<PostViewModel>
            {
                Data = await _mediator.Send(new UpdatePostCommand(post, updatePostDto)),
            });
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
            Post post = await _mediator.Send(new GetPostEntityQuery(postId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, post, PolicyConstants.DeletePostRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            await _mediator.Send(new DeletePostCommand(post));

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

            await _mediator.Send(new VotePostCommand(userId, postId, postVoteDto));

            return Ok();
        }
    }
}