using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;
        private readonly ITokenManager _tokenManager;

        public CategoryController(ICategoryService categoryService, IPostService postService,
            ITokenManager tokenManager)
        {
            _categoryService = categoryService;
            _postService = postService;
            _tokenManager = tokenManager;
        }

        [HttpGet("Category/{name}")]
        public async Task<IActionResult> Details(string name, [Bind("CurrentPage,Limit")] Paginator paginator)
        {
            CategoryViewModel category = _categoryService.GetCategoryByName(name);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(new SearchParams
            {
                CategoryName = category.Name,
            }, paginator);

            var categoryPageViewModel = new CategoryPageViewModel
            {
                Category = category,
                Posts = posts,
            };

            return View(categoryPageViewModel);
        }

        public async Task<IActionResult> Index([Bind("UserId,CategoryName")] SearchParams searchParams,
            [Bind("CurrentPage,Limit")] Paginator paginator)
        {
            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategories(searchParams, paginator);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            return View(categories);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;
            // TODO: populate model with categories
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Image,AuthorId")]
            CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }

            CategoryViewModel category = await _categoryService.CreateCategory(categoryDto);

            return RedirectToAction(nameof(Details), new {category.Name});
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            CategoryViewModel category = _categoryService.GetCategoryById(id);

            if (category.Author.Id != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            return View(category);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
            [Bind("Id,Name,Description,Image")] UpdateCategoryDto updateCategoryDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            CategoryViewModel category = await _categoryService.UpdateCategory(id, userId, updateCategoryDto);

            return RedirectToAction(nameof(Details), new {category.Name});
        }

        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            CategoryViewModel category = _categoryService.GetCategoryById(id);

            if (category.Author.Id != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(new SearchParams
            {
                CategoryName = category.Name,
            }, new Paginator());

            if (posts.Any())
            {
                // throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                //     "Can not delete category which contains posts");
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id,
            [Bind("Id,Name")] DeleteCategoryDto deleteCategoryDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _categoryService.DeleteCategory(id, userId, deleteCategoryDto);

            return RedirectToAction(nameof(Index));
        }
    }
}