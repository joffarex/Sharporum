using MediatR;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Queries.User
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