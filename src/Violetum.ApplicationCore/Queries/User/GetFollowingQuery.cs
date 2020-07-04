using MediatR;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Queries.User
{
    public class GetFollowingQuery : IRequest<UserFollowingViewModel>
    {
        public GetFollowingQuery(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}