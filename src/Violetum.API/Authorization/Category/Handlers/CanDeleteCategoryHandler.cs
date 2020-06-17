using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class
        CanDeleteCategoryHandler : AuthorizationHandler<CanDeleteCategoryAuthorizationRequirement,
            Domain.Entities.Category>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCategoryAuthorizationRequirement requirement, Domain.Entities.Category category)
        {
            string roleBase = $"{nameof(Category)}/{category.Id}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                CategoryHelpers.UserOwnsCategory(context.User.FindFirstValue("sub"), category.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}