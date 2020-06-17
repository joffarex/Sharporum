using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Post.Requirements;
using Violetum.ApplicationCore.Helpers;

namespace Violetum.API.Authorization.Post.Handlers
{
    public class
        CanUpdatePostHandler : AuthorizationHandler<CanUpdatePostAuthorizationRequirement, Domain.Entities.Post>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdatePostAuthorizationRequirement requirement,
            Domain.Entities.Post post)
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