using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sharporum.API.Authorization.Comment.Requirements;
using Sharporum.Core.Helpers;

namespace Sharporum.API.Authorization.Comment.Handlers
{
    public class
        CanUpdateCommentHandler : AuthorizationHandler<CanUpdateCommentAuthorizationRequirement,
            Sharporum.Domain.Entities.Comment
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdateCommentAuthorizationRequirement requirement,
            Sharporum.Domain.Entities.Comment comment)
        {
            if (CommentHelpers.UserOwnsComment(context.User.FindFirstValue("sub"), comment.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}