using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class CanDeleteCategoryHandler : BaseCategoryAuthorizationHandler<CanDeleteCategoryAuthorizationRequirement>
    {
        public CanDeleteCategoryHandler(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCategoryAuthorizationRequirement requirement)
        {
            if (context.User.HasClaim(JwtClaimTypes.Role, $"{RoleBase}/{Roles.Admin}"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}