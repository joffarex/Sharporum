using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Contracts.V1;
using Violetum.API.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class CategoriesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ICategoryService _categoryService;
        private readonly IIdentityManager _identityManager;

        public CategoriesController(ICategoryService categoryService, IIdentityManager identityManager,
            IAuthorizationService authorizationService)
        {
            _categoryService = categoryService;
            _identityManager = identityManager;
            _authorizationService = authorizationService;
        }

        [HttpGet(ApiRoutes.Categories.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] CategorySearchParams searchParams)
        {
            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategories(searchParams);
            int categoriesCount = await _categoryService.GetTotalCategoriesCount(searchParams);

            return Ok(new GetManyResponse<CategoryViewModel>
            {
                Data = categories,
                Count = categoriesCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
            });
        }

        [HttpPost(ApiRoutes.Categories.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            string userId = _identityManager.GetUserId();
            createCategoryDto.AuthorId = userId;

            CategoryViewModel category = await _categoryService.CreateCategory(createCategoryDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new CategoryResponse {Category = category});
        }

        [HttpGet(ApiRoutes.Categories.Get)]
        public IActionResult Get([FromRoute] string categoryId)
        {
            return Ok(new CategoryResponse {Category = _categoryService.GetCategoryById(categoryId)});
        }

        [HttpPut(ApiRoutes.Categories.Update)]
        public async Task<IActionResult> Update([FromRoute] string categoryId,
            [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            CategoryViewModel categoryViewModel = _categoryService.GetCategoryById(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, categoryViewModel,
                    PolicyConstants.UpdateCategoryRolePolicy);
            if (authorizationResult.Succeeded)
            {
                CategoryViewModel category =
                    await _categoryService.UpdateCategory(categoryViewModel, updateCategoryDto);

                return Ok(new CategoryResponse {Category = category});
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }

        [HttpDelete(ApiRoutes.Categories.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string categoryId)
        {
            CategoryViewModel category = _categoryService.GetCategoryById(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, category, PolicyConstants.DeleteCategoryRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _categoryService.DeleteCategory(category);

                return Ok(new ActionSuccessResponse {Message = "OK"});
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }
    }
}