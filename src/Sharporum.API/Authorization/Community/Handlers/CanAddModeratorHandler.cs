using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Sharporum.API.Authorization.Community.Requirements;
using Sharporum.Domain.Models;

namespace Sharporum.API.Authorization.Community.Handlers
{
    public class
        CanAddModeratorHandler : AuthorizationHandler<CanAddModeratorAuthorizationRequirement,
            Sharporum.Domain.Entities.Community
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanAddModeratorAuthorizationRequirement requirement,
            Sharporum.Domain.Entities.Community community)
        {
            string roleBase = $"{nameof(Community)}/{community.Id}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}