using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Post.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Post.Handlers
{
    public class CanDeletePostHandler : AuthorizationHandler<CanDeletePostAuthorizationRequirement, PostViewModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeletePostAuthorizationRequirement requirement,
            PostViewModel post)
        {
            string roleBase = $"{nameof(Category)}/{post.Category.Id}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Moderator}") ||
                PostHelpers.UserOwnsPost(context.User.FindFirstValue("sub"), post.Author.Id))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}