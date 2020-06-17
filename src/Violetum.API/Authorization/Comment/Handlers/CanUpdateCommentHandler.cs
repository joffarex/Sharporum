using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Comment.Requirements;
using Violetum.ApplicationCore.Helpers;

namespace Violetum.API.Authorization.Comment.Handlers
{
    public class
        CanUpdateCommentHandler : AuthorizationHandler<CanUpdateCommentAuthorizationRequirement, Domain.Entities.Comment
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanUpdateCommentAuthorizationRequirement requirement,
            Domain.Entities.Comment comment)
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