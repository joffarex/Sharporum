using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Sharporum.API.Authorization.Community.Requirements;
using Sharporum.Core.Helpers;
using Sharporum.Domain.Models;

namespace Sharporum.API.Authorization.Community.Handlers
{
    public class
        CanDeleteCommunityHandler : AuthorizationHandler<CanDeleteCommunityAuthorizationRequirement,
            Sharporum.Domain.Entities.Community>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCommunityAuthorizationRequirement requirement, Sharporum.Domain.Entities.Community community)
        {
            string roleBase = $"{nameof(Community)}/{community.Id}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                CommunityHelpers.UserOwnsCommunity(context.User.FindFirstValue("sub"), community.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}