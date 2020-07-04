using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Contracts.V1;
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Queries.Community;
using Violetum.ApplicationCore.Responses;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class CommunitiesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly HttpContext _httpContext;
        private readonly IMediator _mediator;

        public CommunitiesController(IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService, IMediator mediator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _authorizationService = authorizationService;
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns communities
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns communities</response>
        /// <response code="404">Unable to find user with provided "UserId"</response>
        [HttpGet(ApiRoutes.Communities.GetMany)]
        [Cached(120)]
        [ProducesResponseType(typeof(FilteredResponse<CommunityViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CommunitySearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out ErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            FilteredDataViewModel<CommunityViewModel> result =
                await _mediator.Send(new GetCommunitiesQuery(searchParams));

            return Ok(new FilteredResponse<CommunityViewModel>(searchParams)
            {
                Data = result.Data,
                Count = result.Count,
            });
        }

        /// <summary>
        ///     Creates community
        /// </summary>
        /// <param name="createCommunityDto"></param>
        /// <response code="200">Creates community</response>
        /// <response code="400">Unable to create community due to validation errors</response>
        /// <response code="404">Unable to find user with provided "AuthorId"</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(ApiRoutes.Communities.Create)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommunityDto createCommunityDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            string communityId = await _mediator.Send(new CreateCommunityCommand(userId, createCommunityDto));

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{communityId}", null);
        }

        /// <summary>
        ///     Returns community
        /// </summary>
        /// <param name="communityId"></param>
        /// <response code="200">Returns community</response>
        /// <response code="404">Unable to find community with provided "communityId"</response>
        [HttpGet(ApiRoutes.Communities.Get)]
        [Cached(120)]
        [ProducesResponseType(typeof(Response<CommunityViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string communityId)
        {
            return Ok(new Response<CommunityViewModel>
            {
                Data = await _mediator.Send(new GetCommunityQuery(communityId)),
            });
        }

        /// <summary>
        ///     Updates community
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="updateCommunityDto"></param>
        /// <response code="200">Updates community</response>
        /// <response code="400">Unable to update community due to validation errors</response>
        [HttpPut(ApiRoutes.Communities.Update)]
        [ProducesResponseType(typeof(Response<CommunityViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromRoute] string communityId,
            [FromBody] UpdateCommunityDto updateCommunityDto)
        {
            Community community = await _mediator.Send(new GetCommunityEntityQuery(communityId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community,
                    PolicyConstants.UpdateCommunityRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            return Ok(new Response<CommunityViewModel>
            {
                Data = await _mediator.Send(new UpdateCommunityCommand(communityId, community, updateCommunityDto)),
            });
        }

        /// <summary>
        ///     Updates community image
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="updateCommunityImageDto"></param>
        /// <response code="200">Updates community image</response>
        /// <response code="400">Unable to update community due to validation errors</response>
        [HttpPut(ApiRoutes.Communities.UpdateImage)]
        [ProducesResponseType(typeof(Response<CommunityViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateImage([FromRoute] string communityId,
            [FromBody] UpdateCommunityImageDto updateCommunityImageDto)
        {
            Community community = await _mediator.Send(new GetCommunityEntityQuery(communityId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community,
                    PolicyConstants.UpdateCommunityRolePolicy);
            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            return Ok(new Response<CommunityViewModel>
            {
                Data = await _mediator.Send(new UpdateCommunityImageCommand(community, updateCommunityImageDto)),
            });
        }

        /// <summary>
        ///     Deletes community
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        /// <response code="200">Deletes community</response>
        /// <response code="400">Unable to delete community due to role removal process</response>
        [HttpDelete(ApiRoutes.Communities.Delete)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] string communityId)
        {
            Community community = await _mediator.Send(new GetCommunityEntityQuery(communityId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community, PolicyConstants.DeleteCommunityRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            await _mediator.Send(new DeleteCommunityCommand(community));

            return Ok();
        }

        /// <summary>
        ///     Adds moderator
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="addModeratorDto"></param>
        /// <response code="200">Updates community</response>
        /// <response code="404">Unable to find user with provided "NewModeratorId"</response>
        [HttpPost(ApiRoutes.Communities.SetModerator)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddModerator([FromRoute] string communityId,
            [FromBody] AddModeratorDto addModeratorDto)
        {
            Community community = await _mediator.Send(new GetCommunityEntityQuery(communityId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community, PolicyConstants.AddModeratorRolePolicy);
            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            await _mediator.Send(new AddModeratorCommand(community, addModeratorDto));

            return Ok();
        }
    }
}