using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
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

        /// <summary>
        ///     Returns categories
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns categories</response>
        /// <response code="404">Unable to find user with provided "UserId"</response>
        [HttpGet(ApiRoutes.Categories.GetMany)]
        [ProducesResponseType(typeof(GetManyResponse<CategoryViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
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

        /// <summary>
        ///     Creates category
        /// </summary>
        /// <param name="createCategoryDto"></param>
        /// <response code="200">Creates category</response>
        /// <response code="400">Unable to create category due to validation errors</response>
        /// <response code="404">Unable to find user with provided "AuthorId"</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.Categories.Create)]
        [ProducesResponseType(typeof(CategoryResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            string userId = _identityManager.GetUserId();

            CategoryViewModel category = await _categoryService.CreateCategory(userId, createCategoryDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new CategoryResponse {Category = category});
        }

        /// <summary>
        ///     Returns category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <response code="200">Returns category</response>
        /// <response code="404">Unable to find category with provided "categoryId"</response>
        [HttpGet(ApiRoutes.Categories.Get)]
        [ProducesResponseType(typeof(CategoryResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public IActionResult Get([FromRoute] string categoryId)
        {
            return Ok(new CategoryResponse {Category = _categoryService.GetCategoryById(categoryId)});
        }

        /// <summary>
        ///     Updates category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="updateCategoryDto"></param>
        /// <response code="200">Updates category</response>
        /// <response code="400">Unable to update category due to validation errors</response>
        [HttpPut(ApiRoutes.Categories.Update)]
        [ProducesResponseType(typeof(CategoryResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
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

        /// <summary>
        ///     Deletes category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        /// <response code="200">Updates category</response>
        /// <response code="400">Unable to delete category due to role removal process</response>
        [HttpDelete(ApiRoutes.Categories.Delete)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
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

        /// <summary>
        ///     Adds moderator
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="addModeratorDto"></param>
        /// <response code="200">Updates category</response>
        /// <response code="404">Unable to find user with provided "NewModeratorId"</response>
        [HttpPost(ApiRoutes.Categories.SetModerator)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddModerator([FromRoute] string categoryId,
            [FromBody] AddModeratorDto addModeratorDto)
        {
            CategoryViewModel category = _categoryService.GetCategoryById(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, category, PolicyConstants.AddModeratorRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _categoryService.AddModerator(category, addModeratorDto);

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