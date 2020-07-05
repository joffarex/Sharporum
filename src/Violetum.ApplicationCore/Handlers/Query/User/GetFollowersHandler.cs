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
        private readonly IUserService _userService;

        public GetFollowersHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserFollowersViewModel> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserFollowersAsync(request.UserId);
        }
    }
}