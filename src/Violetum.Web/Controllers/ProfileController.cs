using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IPostService _postService;
        private readonly ITokenManager _tokenManager;

        public ProfileController(IHttpClientFactory httpClientFactory, ITokenManager tokenManager,
            IPostService postService)
        {
            _tokenManager = tokenManager;
            _postService = postService;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("Profile/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                ProfileViewModel profile = await GetProfileFromIdentityServer(id);
                if (profile == null)
                {
                    return BadRequest();
                }

                IEnumerable<PostViewModel> posts = await GetUserPosts(new SearchParams
                {
                    UserId = profile.Id,
                }, new Paginator());

                string userId = await _tokenManager.GetUserIdFromAccessToken();
                ViewData["UserId"] = userId;

                return View(new ProfilePageViewModel
                {
                    Profile = profile,
                    Posts = posts,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        private async Task<ProfileViewModel> GetProfileFromIdentityServer(string id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5000/Account/{id}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ProfileViewModel>(content);
            }

            return null;
        }

        private async Task<IEnumerable<PostViewModel>> GetUserPosts(SearchParams searchParams, Paginator paginator)
        {
            return await _postService.GetPosts(searchParams, paginator);
        }
    }
}