using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.User
{
    public class FollowUserHandler : IRequestHandler<FollowUserCommand>
    {
        private readonly IFollowerService _followerService;

        public FollowUserHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            await _followerService.FollowUserAsync(request.UserId, request.UserToFollowId);
            return Unit.Value;
        }
    }
}