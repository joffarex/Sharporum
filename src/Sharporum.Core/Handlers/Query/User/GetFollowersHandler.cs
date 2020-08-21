using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.User;
using Sharporum.Core.ViewModels.Follower;

namespace Sharporum.Core.Handlers.Query.User
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