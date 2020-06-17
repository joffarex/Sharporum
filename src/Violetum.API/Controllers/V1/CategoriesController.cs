using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IBlobService _blobService;
        private readonly ICategoryService _categoryService;
        private readonly HttpContext _httpContext;

        public CategoriesController(ICategoryService categoryService, IBlobService blobService,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _categoryService = categoryService;
            _blobService = blobService;
            _httpContext = httpContextAccessor.HttpContext;
            _authorizationService = authorizationService;
        }

        /// <summary>
        ///     Returns categories
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns categories</response>
        /// <response code="404">Unable to find user with provided "UserId"</response>
        [HttpGet(ApiRoutes.Categories.GetMany)]
        [Cached(120)]
        [ProducesResponseType(typeof(GetManyResponse<CategoryViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CategorySearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategoriesAsync(searchParams);
            int categoriesCount = await _categoryService.GetCategoriesCountAsync(searchParams);

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
        [ProducesResponseType(typeof(CreatedResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            string categoryId = await _categoryService.CreateCategoryAsync(userId, createCategoryDto);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{categoryId}",
                new CreatedResponse {Id = categoryId});
        }

        /// <summary>
        ///     Returns category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <response code="200">Returns category</response>
        /// <response code="404">Unable to find category with provided "categoryId"</response>
        [HttpGet(ApiRoutes.Categories.Get)]
        [Cached(120)]
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
            Category category = _categoryService.GetCategoryEntity(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, category,
                    PolicyConstants.UpdateCategoryRolePolicy);
            if (authorizationResult.Succeeded)
            {
                CategoryViewModel categoryViewModel =
                    await _categoryService.UpdateCategoryAsync(category, updateCategoryDto);

                return Ok(new CategoryResponse {Category = categoryViewModel});
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }

        /// <summary>
        ///     Updates category image
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="updateCategoryImageDto"></param>
        /// <response code="200">Updates category image</response>
        /// <response code="400">Unable to update category due to validation errors</response>
        [HttpPut(ApiRoutes.Categories.UpdateImage)]
        [ProducesResponseType(typeof(CategoryResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateImage([FromRoute] string categoryId,
            [FromBody] UpdateCategoryImageDto updateCategoryImageDto)
        {
            Category category = _categoryService.GetCategoryEntity(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, category,
                    PolicyConstants.UpdateCategoryRolePolicy);
            if (authorizationResult.Succeeded)
            {
                FileData data = BaseHelpers.GetFileData<Category>(updateCategoryImageDto.Image, category.Id);
                await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
                updateCategoryImageDto.Image = data.FileName;

                CategoryViewModel categoryViewModel =
                    await _categoryService.UpdateCategoryImageAsync(category, updateCategoryImageDto);

                return Ok(new CategoryResponse {Category = categoryViewModel});
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }

        /// <summary>
        ///     Deletes category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        /// <response code="200">Deletes category</response>
        /// <response code="400">Unable to delete category due to role removal process</response>
        [HttpDelete(ApiRoutes.Categories.Delete)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] string categoryId)
        {
            Category category = _categoryService.GetCategoryEntity(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, category, PolicyConstants.DeleteCategoryRolePolicy);
            if (authorizationResult.Succeeded)
            {
                if (!category.Image.Equals("Category/no-image.jpg"))
                {
                    await _blobService.DeleteBlobAsync(category.Image);
                }

                await _categoryService.DeleteCategoryAsync(category);

                return Ok();
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
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
            Category category = _categoryService.GetCategoryEntity(categoryId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, category, PolicyConstants.AddModeratorRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _categoryService.AddModeratorAsync(category, addModeratorDto);

                return Ok();
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }
    }
}