using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;
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
        public async Task<IActionResult> Details(string name, string postSortBy, string postDir, int postPage,
            string postTitle)
        {
            ViewData["SortByParm"] = string.IsNullOrEmpty(postSortBy) ? "CreatedAt" : postSortBy;
            ViewData["OrderByDirParm"] = string.IsNullOrEmpty(postDir) ? "desc" : postDir;
            ViewData["CurrentPageParm"] = postPage != 0 ? postPage : 1;
            ViewData["PostTitleParm"] = postTitle;

            CategoryViewModel category = _categoryService.GetCategoryByName(name);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            var searchParams = new PostSearchParams
            {
                SortBy = (string) ViewData["SortByParm"],
                OrderByDir = (string) ViewData["OrderByDirParm"],
                CurrentPage = (int) ViewData["CurrentPageParm"],
                CategoryName = category.Name,
            };

            if (!string.IsNullOrEmpty(postTitle))
            {
                searchParams.PostTitle = postTitle;
            }

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams);
            int totalPosts = await _postService.GetTotalPostsCount(searchParams);
            ViewData["TotalPosts"] = totalPosts;

            var totalPages =
                (int) Math.Ceiling(totalPosts / (double) searchParams.Limit);
            ViewData["totalPages"] = totalPages;

            var categoryPageViewModel = new CategoryPageViewModel
            {
                Category = category,
                Posts = posts,
            };

            return View(categoryPageViewModel);
        }

        public async Task<IActionResult> Index(string name)
        {
            ViewData["NameParm"] = name;

            var searchParams = new CategorySearchParams();

            if (!string.IsNullOrEmpty(name))
            {
                searchParams.CategoryName = name;
            }

            IEnumerable<CategoryViewModel>
                categories = await _categoryService.GetCategories(searchParams);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            return View(categories);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            return View(new CategoryDto());
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

            try
            {
                CategoryViewModel category = await _categoryService.CreateCategory(categoryDto);

                TempData["CreateCategorySuccess"] = "Category successfully created";
                return RedirectToAction(nameof(Details), new {category.Name});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
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

            return View(new UpdateCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Image = category.Image,
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
            [Bind("Id,Name,Description,Image")] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit));
            }

            string userId = await _tokenManager.GetUserIdFromAccessToken();

            try
            {
                CategoryViewModel category = await _categoryService.UpdateCategory(id, userId, updateCategoryDto);

                TempData["EditCategorySuccess"] = "Category successfully edited";
                return RedirectToAction(nameof(Details), new {category.Name});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
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

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(new PostSearchParams
            {
                CategoryName = category.Name,
                OrderByDir = "asc",
                SortBy = "CreatedAt",
            });

            if (!posts.Any())
            {
                return View(category);
            }

            TempData["DeleteCategoryFailure"] =
                $"Can not delete category which contains posts. Category Name - {category.Name}";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            try
            {
                await _categoryService.DeleteCategory(id, userId);

                TempData["DeleteCategorySuccess"] = "Category successfully deleted";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}