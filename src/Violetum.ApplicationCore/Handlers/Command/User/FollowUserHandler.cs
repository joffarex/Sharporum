using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.User
{
    public class FollowUserHandler : IRequestHandler<FollowUserCommand>
    {
        private readonly IUserService _userService;

        public FollowUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            await _userService.FollowUserAsync(request.UserId, request.UserToFollowId);
            return Unit.Value;
        }
    }
}