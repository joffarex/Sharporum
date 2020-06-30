using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class CanManageCategoryHandler : AuthorizationHandler<CanManageCategoryAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanManageCategoryAuthorizationRequirement requirement)
        {
            if (context.User.HasClaim(JwtClaimTypes.Role, Roles.SuperAdmin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}