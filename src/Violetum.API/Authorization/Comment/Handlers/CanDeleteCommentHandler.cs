using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Comment.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Comment.Handlers
{
    public class
        CanDeleteCommentHandler : AuthorizationHandler<CanDeleteCommentAuthorizationRequirement, Domain.Entities.Comment
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCommentAuthorizationRequirement requirement,
            Domain.Entities.Comment comment)
        {
            string roleBase = $"{nameof(Category)}/{comment.Post.CategoryId}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Moderator}") ||
                CommentHelpers.UserOwnsComment(context.User.FindFirstValue("sub"), comment.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}