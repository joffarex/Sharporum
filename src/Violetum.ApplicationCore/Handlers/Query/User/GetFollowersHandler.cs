using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.User;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetFollowersHandler : IRequestHandler<GetFollowersQuery, FollowersResponse<UserFollowersViewModel>>
    {
        private readonly IFollowerService _followerService;

        public GetFollowersHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<FollowersResponse<UserFollowersViewModel>> Handle(GetFollowersQuery request,
            CancellationToken cancellationToken)
        {
            return new FollowersResponse<UserFollowersViewModel>
            {
                Followers = await _followerService.GetUserFollowersAsync(request.UserId),
            };
        }
    }
}