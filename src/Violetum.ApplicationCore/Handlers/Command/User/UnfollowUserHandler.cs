using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.User
{
    public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand>
    {
        private readonly IFollowerService _followerService;

        public UnfollowUserHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<Unit> Handle(UnfollowUserCommand request,
            CancellationToken cancellationToken)
        {
            await _followerService.UnfollowUserAsync(request.UserId, request.UserToUnfollowId);
            return Unit.Value;
        }
    }
}