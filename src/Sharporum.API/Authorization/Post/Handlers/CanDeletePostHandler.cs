using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Sharporum.API.Authorization.Post.Requirements;
using Sharporum.Core.Helpers;
using Sharporum.Domain.Models;

namespace Sharporum.API.Authorization.Post.Handlers
{
    public class
        CanDeletePostHandler : AuthorizationHandler<CanDeletePostAuthorizationRequirement, Sharporum.Domain.Entities.Post
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeletePostAuthorizationRequirement requirement,
            Sharporum.Domain.Entities.Post post)
        {
            string roleBase = $"{nameof(Community)}/{post.CommunityId}";

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