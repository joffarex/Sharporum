using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Post.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Post.Handlers
{
    public class
        CanDeletePostHandler : AuthorizationHandler<CanDeletePostAuthorizationRequirement, Domain.Entities.Post>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeletePostAuthorizationRequirement requirement,
            Domain.Entities.Post post)
        {
            string roleBase = $"{nameof(Category)}/{post.CategoryId}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Moderator}") ||
                PostHelpers.UserOwnsPost(context.User.FindFirstValue("sub"), post.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}