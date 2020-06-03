using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.API.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ITokenManager _tokenManager;

        public PostsController(IPostService postService, ITokenManager tokenManager)
        {
            _postService = postService;
            _tokenManager = tokenManager;
        }

        [HttpGet("/secret")]
        [Authorize]
        public IActionResult Secret()
        {
            return Ok(new {Msg = "Secret"});
        }

        [HttpGet(ApiRoutes.Posts.GetMany)]
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

        [HttpPost(ApiRoutes.Posts.Create)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            createPostDto.AuthorId = userId;

            PostViewModel post = await _postService.CreatePost(createPostDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new PostResponse {Post = post});
        }

        [HttpGet(ApiRoutes.Posts.NewsFeed)]
        [Authorize]
        public async Task<IActionResult> NewsFeed([FromQuery] PostSearchParams searchParams)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(userId, searchParams);
            int postsCount = _postService.GetTotalPostsCountInNewsFeed(userId, searchParams);

            return Ok(new GetManyResponse<PostViewModel>
            {
                Data = posts,
                Count = postsCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
            });
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute] string postId)
        {
            return Ok(new PostResponse {Post = _postService.GetPost(postId)});
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] string postId, [FromBody] UpdatePostDto updatePostDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            PostViewModel post = await _postService.UpdatePost(postId, userId, updatePostDto);

            return Ok(new PostResponse {Post = post});
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string postId)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _postService.DeletePost(postId, userId);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }

        [HttpPost(ApiRoutes.Posts.Vote)]
        [Authorize]
        public async Task<IActionResult> Vote([FromRoute] string postId, [FromBody] PostVoteDto postVoteDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _postService.VotePost(postId, userId, postVoteDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}