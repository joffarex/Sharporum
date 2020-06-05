using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class
        CanUpdateCategoryHandler : BaseCategoryAuthorizationHandler<CanUpdateCategoryAuthorizationRequirement>
    {
        public CanUpdateCategoryHandler(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdateCategoryAuthorizationRequirement requirement)
        {
            if (context.User.HasClaim(JwtClaimTypes.Role, $"{RoleBase}/{Roles.Admin}") ||
                context.User.HasClaim(JwtClaimTypes.Role, $"{RoleBase}/{Roles.Moderator}"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}