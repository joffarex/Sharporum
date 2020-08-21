using MediatR;
using Sharporum.Core.ViewModels.Follower;

namespace Sharporum.Core.Queries.User
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