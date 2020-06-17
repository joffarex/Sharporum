using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class
        CanAddModeratorHandler : AuthorizationHandler<CanAddModeratorAuthorizationRequirement, Domain.Entities.Category>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanAddModeratorAuthorizationRequirement requirement,
            Domain.Entities.Category category)
        {
            string roleBase = $"{nameof(Category)}/{category.Id}";

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