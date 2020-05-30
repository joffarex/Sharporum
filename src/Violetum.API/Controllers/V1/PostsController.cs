using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.Domain.Entities;

namespace Violetum.API.Controllers.V1
{
    public class PostsController : ControllerBase
    {
        private readonly List<Post> _posts;

        public PostsController()
        {
            _posts = new List<Post>();
            _posts.Add(new Post {Title = "Test"});
            _posts.Add(new Post {Title = "Test2"});
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(new {Posts = _posts});
        }
    }
}