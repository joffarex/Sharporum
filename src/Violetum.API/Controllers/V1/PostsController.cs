using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Contracts.V1;
using Violetum.API.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityManager _identityManager;
        private readonly IPostService _postService;

        public PostsController(IPostService postService, IIdentityManager identityManager,
            IAuthorizationService authorizationService)
        {
            _postService = postService;
            _identityManager = identityManager;
            _authorizationService = authorizationService;
        }

        [HttpGet("/secret")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Secret()
        {
            string userId = _identityManager.GetUserId();
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
        [ProducesResponseType(typeof(GetManyResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] PostSearchParams searchParams)
        {
            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams);
            int postsCount = await _postService.GetTotalPostsCount(searchParams);

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
        [ProducesResponseType(typeof(PostResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
            string userId = _identityManager.GetUserId();
            createPostDto.AuthorId = userId;

            PostViewModel post = await _postService.CreatePost(createPostDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new PostResponse {Post = post});
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(GetManyResponse<PostViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public IActionResult NewsFeed([FromQuery] PostSearchParams searchParams)
        {
            string userId = _identityManager.GetUserId();

            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(userId, searchParams);
            int postsCount = _postService.GetTotalPostsCountInNewsFeed(userId, searchParams);

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
            PostViewModel postViewModel = _postService.GetPost(postId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, postViewModel, PolicyConstants.UpdatePostRolePolicy);
            if (authorizationResult.Succeeded)
            {
                PostViewModel post = await _postService.UpdatePost(postViewModel, updatePostDto);

                return Ok(new PostResponse {Post = post});
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }

        /// <summary>
        ///     Deletes post
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="200">Deletes post</response>
        [HttpDelete(ApiRoutes.Posts.Delete)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> Delete([FromRoute] string postId)
        {
            PostViewModel post = _postService.GetPost(postId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, post, PolicyConstants.DeletePostRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _postService.DeletePost(post);

                return Ok(new ActionSuccessResponse {Message = "OK"});
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
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
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Vote([FromRoute] string postId, [FromBody] PostVoteDto postVoteDto)
        {
            string userId = _identityManager.GetUserId();

            await _postService.VotePost(postId, userId, postVoteDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}