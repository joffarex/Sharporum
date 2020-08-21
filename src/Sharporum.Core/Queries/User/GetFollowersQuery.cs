using MediatR;
using Sharporum.Core.ViewModels.Follower;

namespace Sharporum.Core.Queries.User
{
    public class GetFollowersQuery : IRequest<UserFollowersViewModel>
    {
        public GetFollowersQuery(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}