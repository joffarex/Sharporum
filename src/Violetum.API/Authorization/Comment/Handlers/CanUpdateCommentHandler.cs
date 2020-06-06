using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Comment.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.API.Authorization.Comment.Handlers
{
    public class
        CanUpdateCommentHandler : AuthorizationHandler<CanUpdateCommentAuthorizationRequirement, CommentViewModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdateCommentAuthorizationRequirement requirement,
            CommentViewModel comment)
        {
            if (PostHelpers.UserOwnsPost(context.User.FindFirstValue("sub"), comment.Author.Id))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}