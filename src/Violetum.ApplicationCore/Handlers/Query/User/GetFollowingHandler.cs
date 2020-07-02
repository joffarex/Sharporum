﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.User;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetFollowingHandler : IRequestHandler<GetFollowingQuery, FollowersResponse<UserFollowingViewModel>>
    {
        private readonly IFollowerService _followerService;

        public GetFollowingHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<FollowersResponse<UserFollowingViewModel>> Handle(GetFollowingQuery request,
            CancellationToken cancellationToken)
        {
            return new FollowersResponse<UserFollowingViewModel>
            {
                Followers = await _followerService.GetUserFollowingAsync(request.UserId),
            };
        }
    }
}