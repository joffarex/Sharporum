using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.User;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetFollowersHandler : IRequestHandler<GetFollowersQuery, UserFollowersViewModel>
    {
        private readonly IFollowerService _followerService;

        public GetFollowersHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<UserFollowersViewModel> Handle(GetFollowersQuery request,
            CancellationToken cancellationToken)
        {
            return await _followerService.GetUserFollowersAsync(request.UserId);
        }
    }
}