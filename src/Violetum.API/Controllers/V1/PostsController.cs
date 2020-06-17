using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
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
        private readonly IPostService _postService;

        public PostsController(IPostService postService, IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _postService = postService;
            _httpContext = httpContextAccessor.HttpContext;
            _authorizationService = authorizationService;
        }

        [HttpGet("/secret")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Secret()
        {
            string userId = _httpContext.User.FindFirstValue("sub");
            return Ok(new {Msg = "Secret", UserId = userId});
        }

        /// <summary>
        ///     Returns posts
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns posts</response>
        /// <response code="404">
        ///     Unable to find user with provided "UserId" / category with provided "CategoryName" / post with
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

            IEnumerable<PostViewModel> posts = await _postService.GetPostsAsync(searchParams);
            int postsCount = await _postService.GetPostsCountAsync(searchParams);

            return Ok(new GetManyResponse<PostViewModel>
            {
                Data = posts,
                Count = postsCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
            });
        }

        /// <summary>
        ///     Creates post
        /// </summary>
        /// <param name="createPostDto"></param>
        /// <response code="200">Creates post</response>
        /// <response code="400">Unable to create post due to validation errors</response>
        /// <response code="404">Unable to find user with provided "AuthorId"/ category with provided "CategoryId"</response>
        [HttpPost(ApiRoutes.Posts.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(CreatedResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            string postId = await _postService.CreatePostAsync(userId, createPostDto);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{postId}", new CreatedResponse {Id = postId});
        }

        /// <summary>
        ///     Returns newsfeed
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns newsfeed</response>
        /// <response code="404">
        ///     Unable to find user with provided "UserId" / category with provided "CategoryName" / post with
        ///     provided "PostTitle
        /// </response>
        [HttpGet(ApiRoutes.Posts.NewsFeed)]
        [Cached(180)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(GetManyResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public IActionResult NewsFeed([FromQuery] PostSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            string userId = _httpContext.User.FindFirstValue("sub");

            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(userId, searchParams);
            int postsCount = _postService.GetPostsCountInNewsFeed(userId, searchParams);

            return Ok(new GetManyResponse<PostViewModel>
            {
                Data = posts,
                Count = postsCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
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
        [ProducesResponseType(typeof(PostResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public IActionResult Get([FromRoute] string postId)
        {
            return Ok(new PostResponse {Post = _postService.GetPost(postId)});
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
            Post post = _postService.GetPostEntity(postId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, post, PolicyConstants.UpdatePostRolePolicy);
            if (authorizationResult.Succeeded)
            {
                PostViewModel postViewModel = await _postService.UpdatePostAsync(post, updatePostDto);

                return Ok(new PostResponse {Post = postViewModel});
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
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
            Post post = _postService.GetPostEntity(postId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, post, PolicyConstants.DeletePostRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _postService.DeletePostAsync(post);

                return Ok();
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
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

            await _postService.VotePostAsync(postId, userId, postVoteDto);

            return Ok();
        }
    }
}