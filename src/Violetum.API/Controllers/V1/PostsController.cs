using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] PostSearchParams searchParams)
        {
            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams);
            int postsCount = await _postService.GetTotalPostsCount(searchParams);

            return Ok(new
            {
                Posts = posts, PostsCount = postsCount, Params = new {searchParams.Limit, searchParams.CurrentPage},
            });
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
            PostViewModel post = await _postService.CreatePost(createPostDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new {Post = post});
        }

        [HttpGet(ApiRoutes.Posts.NewsFeed)]
        public IActionResult NewsFeed([FromQuery] PostSearchParams searchParams)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(userId, searchParams);
            int postsCount = _postService.GetTotalPostsCountInNewsFeed(userId, searchParams);

            return Ok(new
            {
                Posts = posts, PostsCount = postsCount, Params = new {searchParams.Limit, searchParams.CurrentPage},
            });
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute] string postId)
        {
            return Ok(new {Post = _postService.GetPost(postId)});
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] string postId, [FromBody] UpdatePostDto updatePostDto)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            PostViewModel post = await _postService.UpdatePost(postId, userId, updatePostDto);

            return Ok(new {Post = post});
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string postId)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            await _postService.DeletePost(postId, userId);

            return Ok(new {Message = "OK"});
        }

        [HttpPost(ApiRoutes.Posts.Vote)]
        public async Task<IActionResult> Vote([FromRoute] string postId, [FromBody] PostVoteDto postVoteDto)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            await _postService.VotePost(postId, userId, postVoteDto);

            return Ok(new {Message = "OK"});
        }
    }
}