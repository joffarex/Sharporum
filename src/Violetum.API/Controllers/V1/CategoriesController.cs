﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ITokenManager _tokenManager;

        public CategoriesController(ICategoryService categoryService, ITokenManager tokenManager)
        {
            _categoryService = categoryService;
            _tokenManager = tokenManager;
        }

        [HttpGet(ApiRoutes.Categories.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] CategorySearchParams searchParams)
        {
            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategories(searchParams);
            return Ok(new {Categories = categories, Params = new {searchParams.Limit, searchParams.CurrentPage}});
        }

        [HttpPost(ApiRoutes.Categories.Create)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
                        string userId = await _tokenManager.GetUserIdFromAccessToken();
                        createCategoryDto.AuthorId = userId;

            CategoryViewModel category = await _categoryService.CreateCategory(createCategoryDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new {Category = category});
        }

        [HttpGet(ApiRoutes.Categories.Get)]
        public IActionResult Get([FromRoute] string categoryId)
        {
            return Ok(new {Category = _categoryService.GetCategoryById(categoryId)});
        }

        [HttpPut(ApiRoutes.Categories.Update)]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] string categoryId,
            [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            CategoryViewModel category = await _categoryService.UpdateCategory(categoryId, userId, updateCategoryDto);

            return Ok(new {Category = category});
        }

        [HttpDelete(ApiRoutes.Categories.Delete)]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string categoryId)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _categoryService.DeleteCategory(categoryId, userId);

            return Ok(new {Message = "OK"});
        }
    }
}