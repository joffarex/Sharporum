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
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
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
        private readonly IBlobService _blobService;
        private readonly ICommunityService _communityService;
        private readonly HttpContext _httpContext;

        public CommunitiesController(ICommunityService communityService, IBlobService blobService,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            _communityService = communityService;
            _blobService = blobService;
            _httpContext = httpContextAccessor.HttpContext;
            _authorizationService = authorizationService;
        }

        /// <summary>
        ///     Returns communities
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns communities</response>
        /// <response code="404">Unable to find user with provided "UserId"</response>
        [HttpGet(ApiRoutes.Categories.GetMany)]
        [Cached(120)]
        [ProducesResponseType(typeof(GetManyResponse<CommunityViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CommunitySearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            IEnumerable<CommunityViewModel> communities = await _communityService.GetCommunitiesAsync(searchParams);
            int communitiesCount = await _communityService.GetCategoriesCountAsync(searchParams);

            return Ok(new GetManyResponse<CommunityViewModel>
            {
                Data = communities,
                Count = communitiesCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
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
        [HttpPost(ApiRoutes.Categories.Create)]
        [ProducesResponseType(typeof(CreatedResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommunityDto createCommunityDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            string communityId = await _communityService.CreateCommunityAsync(userId, createCommunityDto);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{communityId}",
                new CreatedResponse {Id = communityId});
        }

        /// <summary>
        ///     Returns community
        /// </summary>
        /// <param name="communityId"></param>
        /// <response code="200">Returns community</response>
        /// <response code="404">Unable to find community with provided "communityId"</response>
        [HttpGet(ApiRoutes.Categories.Get)]
        [Cached(120)]
        [ProducesResponseType(typeof(CommunityResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public IActionResult Get([FromRoute] string communityId)
        {
            return Ok(new CommunityResponse {Community = _communityService.GetCommunityById(communityId)});
        }

        /// <summary>
        ///     Updates community
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="updateCommunityDto"></param>
        /// <response code="200">Updates community</response>
        /// <response code="400">Unable to update community due to validation errors</response>
        [HttpPut(ApiRoutes.Categories.Update)]
        [ProducesResponseType(typeof(CommunityResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromRoute] string communityId,
            [FromBody] UpdateCommunityDto updateCommunityDto)
        {
            Community community = _communityService.GetCommunityEntity(communityId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community,
                    PolicyConstants.UpdateCommunityRolePolicy);
            if (authorizationResult.Succeeded)
            {
                CommunityViewModel communityViewModel =
                    await _communityService.UpdateCommunityAsync(community, updateCommunityDto);

                return Ok(new CommunityResponse {Community = communityViewModel});
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }

        /// <summary>
        ///     Updates community image
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="updateCommunityImageDto"></param>
        /// <response code="200">Updates community image</response>
        /// <response code="400">Unable to update community due to validation errors</response>
        [HttpPut(ApiRoutes.Categories.UpdateImage)]
        [ProducesResponseType(typeof(CommunityResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateImage([FromRoute] string communityId,
            [FromBody] UpdateCommunityImageDto updateCommunityImageDto)
        {
            Community community = _communityService.GetCommunityEntity(communityId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community,
                    PolicyConstants.UpdateCommunityRolePolicy);
            if (authorizationResult.Succeeded)
            {
                FileData data = BaseHelpers.GetFileData<Community>(updateCommunityImageDto.Image, community.Id);
                await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
                updateCommunityImageDto.Image = data.FileName;

                CommunityViewModel communityViewModel =
                    await _communityService.UpdateCommunityImageAsync(community, updateCommunityImageDto);

                return Ok(new CommunityResponse {Community = communityViewModel});
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }

        /// <summary>
        ///     Deletes community
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        /// <response code="200">Deletes community</response>
        /// <response code="400">Unable to delete community due to role removal process</response>
        [HttpDelete(ApiRoutes.Categories.Delete)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] string communityId)
        {
            Community community = _communityService.GetCommunityEntity(communityId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community, PolicyConstants.DeleteCommunityRolePolicy);
            if (authorizationResult.Succeeded)
            {
                if (!community.Image.Equals($"{nameof(Community)}/no-image.jpg"))
                {
                    await _blobService.DeleteBlobAsync(community.Image);
                }

                await _communityService.DeleteCommunityAsync(community);

                return Ok();
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }

        /// <summary>
        ///     Adds moderator
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="addModeratorDto"></param>
        /// <response code="200">Updates community</response>
        /// <response code="404">Unable to find user with provided "NewModeratorId"</response>
        [HttpPost(ApiRoutes.Categories.SetModerator)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddModerator([FromRoute] string communityId,
            [FromBody] AddModeratorDto addModeratorDto)
        {
            Community community = _communityService.GetCommunityEntity(communityId);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, community, PolicyConstants.AddModeratorRolePolicy);
            if (authorizationResult.Succeeded)
            {
                await _communityService.AddModeratorAsync(community, addModeratorDto);

                return Ok();
            }

            return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
        }
    }
}