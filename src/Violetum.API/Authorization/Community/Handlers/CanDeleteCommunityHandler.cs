﻿using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Violetum.API.Authorization.Community.Requirements;
using Violetum.ApplicationCore.Helpers;
using Violetum.Domain.Models;

namespace Violetum.API.Authorization.Community.Handlers
{
    public class
        CanDeleteCommunityHandler : AuthorizationHandler<CanDeleteCommunityAuthorizationRequirement,
            Domain.Entities.Community>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanDeleteCommunityAuthorizationRequirement requirement, Domain.Entities.Community community)
        {
            string roleBase = $"{nameof(Community)}/{community.Id}";

            if (context.User.HasClaim(JwtClaimTypes.Role, $"{roleBase}/{Roles.Admin}") ||
                CommunityHelpers.UserOwnsCommunity(context.User.FindFirstValue("sub"), community.AuthorId))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}