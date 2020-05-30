using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet(ApiRoutes.Categories.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] CategorySearchParams searchParams)
        {
            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategories(searchParams);
            return Ok(new {Categories = categories, Params = new {searchParams.Limit, searchParams.CurrentPage}});
        }

        [HttpPost(ApiRoutes.Categories.Create)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            CategoryViewModel category = await _categoryService.CreateCategory(createCategoryDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new {Category = category});
        }

        [HttpGet(ApiRoutes.Categories.Get)]
        public IActionResult Get(string categoryId)
        {
            return Ok(new {Category = _categoryService.GetCategoryById(categoryId)});
        }

        [HttpPut(ApiRoutes.Categories.Update)]
        public async Task<IActionResult> Update([FromRoute] string categoryId,
            [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            CategoryViewModel category = await _categoryService.UpdateCategory(categoryId, userId, updateCategoryDto);

            return Ok(new {Category = category});
        }

        [HttpDelete(ApiRoutes.Categories.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string categoryId)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            await _categoryService.DeleteCategory(categoryId, userId);

            return Ok(new {Message = "OK"});
        }
    }
}