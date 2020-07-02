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
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Queries.Community;
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
        [ProducesResponseType(typeof(GetManyResponse<CommunityViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CommunitySearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            var query = new GetCommunitiesQuery(searchParams);
            GetManyResponse<CommunityViewModel> result = await _mediator.Send(query);

            return Ok(result);
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
        [ProducesResponseType(typeof(CreatedResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommunityDto createCommunityDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new CreateCommunityCommand(userId, createCommunityDto);
            CreatedResponse result = await _mediator.Send(command);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{result.Id}", result);
        }

        /// <summary>
        ///     Returns community
        /// </summary>
        /// <param name="communityId"></param>
        /// <response code="200">Returns community</response>
        /// <response code="404">Unable to find community with provided "communityId"</response>
        [HttpGet(ApiRoutes.Communities.Get)]
        [Cached(120)]
        [ProducesResponseType(typeof(CommunityResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string communityId)
        {
            var query = new GetCommunityQuery(communityId);
            CommunityResponse result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        ///     Updates community
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="updateCommunityDto"></param>
        /// <response code="200">Updates community</response>
        /// <response code="400">Unable to update community due to validation errors</response>
        [HttpPut(ApiRoutes.Communities.Update)]
        [ProducesResponseType(typeof(CommunityResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromRoute] string communityId,
            [FromBody] UpdateCommunityDto updateCommunityDto)
        {
            var query = new GetCommunityEntityQuery(communityId);
            Community community = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community,
                    PolicyConstants.UpdateCommunityRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new UpdateCommunityCommand(communityId, community, updateCommunityDto);
            CommunityResponse result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        ///     Updates community image
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="updateCommunityImageDto"></param>
        /// <response code="200">Updates community image</response>
        /// <response code="400">Unable to update community due to validation errors</response>
        [HttpPut(ApiRoutes.Communities.UpdateImage)]
        [ProducesResponseType(typeof(CommunityResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateImage([FromRoute] string communityId,
            [FromBody] UpdateCommunityImageDto updateCommunityImageDto)
        {
            var query = new GetCommunityEntityQuery(communityId);
            Community community = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community,
                    PolicyConstants.UpdateCommunityRolePolicy);
            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new UpdateCommunityImageCommand(community, updateCommunityImageDto);
            CommunityResponse result = await _mediator.Send(command);

            return Ok(result);
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
            var query = new GetCommunityEntityQuery(communityId);
            Community community = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community, PolicyConstants.DeleteCommunityRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new DeleteCommunityCommand(community);
            await _mediator.Send(command);

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
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddModerator([FromRoute] string communityId,
            [FromBody] AddModeratorDto addModeratorDto)
        {
            var query = new GetCommunityEntityQuery(communityId);
            Community community = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community, PolicyConstants.AddModeratorRolePolicy);
            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new AddModeratorCommand(community, addModeratorDto);
            await _mediator.Send(command);

            return Ok();
        }
    }
}