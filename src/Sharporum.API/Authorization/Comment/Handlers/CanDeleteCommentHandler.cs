using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Sharporum.API.Authorization.Comment.Requirements;
using Sharporum.Core.Helpers;
using Sharporum.Domain.Models;

namespace Sharporum.API.Authorization.Comment.Handlers
{
    public class
        CanDeleteCommentHandler : AuthorizationHandler<CanDeleteCommentAuthorizationRequirement,
            Sharporum.Domain.Entities.Comment
        >
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCommentAuthorizationRequirement requirement,
            Sharporum.Domain.Entities.Comment comment)
        {
            string roleBase = $"{nameof(Community)}/{comment.Post.CommunityId}";

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