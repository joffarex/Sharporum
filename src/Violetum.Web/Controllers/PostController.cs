using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ITokenManager _tokenManager;

        public PostController(IPostService postService, ITokenManager tokenManager)
        {
            _postService = postService;
            _tokenManager = tokenManager;
        }

        [HttpGet("{postId}")]
        public IActionResult GetPost(string postId)
        {
            try
            {
                PostViewModel post = _postService.GetPost(postId);
                return View(post);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetPosts([FromBody] SearchParams searchParams, [FromBody] Paginator paginator)
        {
            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams, paginator);

            return View(posts);
        }

        [Authorize]
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostDto postDto)
        {
            try
            {
                PostViewModel post = await _postService.CreatePost(postDto);

                return RedirectToAction("GetPost", new {PostId = post.Id});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [Authorize]
        [HttpPut("{postId}")]
        public async Task<IActionResult> Update(string postId, [FromBody] UpdatePostDto updatePostDto)
        {
            try
            {
                string userId = await GetUserIdFromAccessToken();

                PostViewModel post = await _postService.UpdatePost(postId, userId, updatePostDto);

                return RedirectToAction("GetPost", new {PostId = post.Id});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete(string postId, [FromBody] DeletePostDto deletePostDto)
        {
            try
            {
                string userId = await GetUserIdFromAccessToken();

                await _postService.DeletePost(postId, userId, deletePostDto);

                return RedirectToAction("GetPosts");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        private async Task<string> GetUserIdFromAccessToken()
        {
            UserTokens tokens = await _tokenManager.GetUserTokens();
            JwtSecurityToken accessToken = new JwtSecurityTokenHandler().ReadJwtToken(tokens.AccessToken);
            return accessToken.Subject;
        }
    }
}