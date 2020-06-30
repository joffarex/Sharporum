using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Community.Requirements;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Community.Handlers
{
    public class
        CanAddModeratorHandler : AuthorizationHandler<CanAddModeratorAuthorizationRequirement, Domain.Entities.Community
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanAddModeratorAuthorizationRequirement requirement,
            Domain.Entities.Community community)
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