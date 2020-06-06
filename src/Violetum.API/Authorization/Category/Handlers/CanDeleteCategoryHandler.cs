using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class
        CanDeleteCategoryHandler : AuthorizationHandler<CanDeleteCategoryAuthorizationRequirement, CategoryViewModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCategoryAuthorizationRequirement requirement, CategoryViewModel category)
        {
            string roleBase = $"{nameof(Category)}/{category.Id}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                CategoryHelpers.UserOwnsCategory(context.User.FindFirstValue("sub"), category.Author.Id))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}