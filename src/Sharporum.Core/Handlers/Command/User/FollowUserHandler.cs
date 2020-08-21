using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.User;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.User
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