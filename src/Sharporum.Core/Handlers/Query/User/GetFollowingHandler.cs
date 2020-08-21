using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.User;
using Sharporum.Core.ViewModels.Follower;

namespace Sharporum.Core.Handlers.Query.User
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