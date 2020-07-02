using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Queries.User
{
    public class GetFollowingQuery : IRequest<FollowersResponse<UserFollowingViewModel>>
    {
        public GetFollowingQuery(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}