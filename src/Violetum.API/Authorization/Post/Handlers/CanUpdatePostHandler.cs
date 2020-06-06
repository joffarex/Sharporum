using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.API.Authorization.Post.Handlers
{
    public class CanUpdatePostHandler : AuthorizationHandler<CanUpdateCategoryAuthorizationRequirement, PostViewModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdateCategoryAuthorizationRequirement requirement,
            PostViewModel post)
        {
            if (PostHelpers.UserOwnsPost(context.User.FindFirstValue("sub"), post.Author.Id))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}