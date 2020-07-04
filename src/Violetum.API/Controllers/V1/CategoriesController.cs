using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Contracts.V1;
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Commands.Category;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Queries.Category;
using Violetum.ApplicationCore.Responses;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMediator _mediator;

        public CategoriesController(IAuthorizationService authorizationService, IMediator mediator)
        {
            _authorizationService = authorizationService;
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns categories
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns categories</response>
        /// <response code="404">Unable to find user with provided "UserId"</response>
        [HttpGet(ApiRoutes.Categories.GetMany)]
        [Cached(120)]
        [ProducesResponseType(typeof(FilteredResponse<CategoryViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CategorySearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out ErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            FilteredDataViewModel<CategoryViewModel>
                result = await _mediator.Send(new GetCategoriesQuery(searchParams));

            return Ok(new FilteredResponse<CategoryViewModel>(searchParams)
            {
                Data = result.Data,
                Count = result.Count,
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
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, PolicyConstants.ManageCategoryRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            string categoryId = await _mediator.Send(new CreateCategoryCommand(createCategoryDto));

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{categoryId}", null);
        }

        /// <summary>
        ///     Returns category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <response code="200">Returns category</response>
        /// <response code="404">Unable to find category with provided "categoryId"</response>
        [HttpGet(ApiRoutes.Categories.Get)]
        [Cached(120)]
        [ProducesResponseType(typeof(Response<CategoryViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string categoryId)
        {
            return Ok(new Response<CategoryViewModel>
            {
                Data = await _mediator.Send(new GetCategoryQuery(categoryId)),
            });
        }

        /// <summary>
        ///     Updates category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="updateCategoryDto"></param>
        /// <response code="200">Updates category</response>
        /// <response code="400">Unable to update category due to validation errors</response>
        [HttpPut(ApiRoutes.Categories.Update)]
        [ProducesResponseType(typeof(Response<CategoryViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromRoute] string categoryId,
            [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, PolicyConstants.ManageCategoryRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            return Ok(new Response<CategoryViewModel>
            {
                Data = await _mediator.Send(new UpdateCategoryCommand(categoryId, updateCategoryDto)),
            });
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
            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, PolicyConstants.ManageCategoryRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            await _mediator.Send(new DeleteCategoryCommand(categoryId));

            return Ok();
        }
    }
}