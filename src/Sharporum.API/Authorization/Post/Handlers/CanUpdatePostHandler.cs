using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sharporum.API.Authorization.Post.Requirements;
using Sharporum.Core.Helpers;

namespace Sharporum.API.Authorization.Post.Handlers
{
    public class
        CanUpdatePostHandler : AuthorizationHandler<CanUpdatePostAuthorizationRequirement, Sharporum.Domain.Entities.Post
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdatePostAuthorizationRequirement requirement,
            Sharporum.Domain.Entities.Post post)
        {
            if (PostHelpers.UserOwnsPost(context.User.FindFirstValue("sub"), post.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}