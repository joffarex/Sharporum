using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.User;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetFollowingHandler : IRequestHandler<GetFollowingQuery, UserFollowingViewModel>
    {
        private readonly IUserService _userService;

        public GetFollowingHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserFollowingViewModel> Handle(GetFollowingQuery request,
            CancellationToken cancellationToken)
        {
            return await _userService.GetUserFollowingAsync(request.UserId);
        }
    }
}