using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.User
{
    public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand, ActionSuccessResponse>
    {
        private readonly IFollowerService _followerService;

        public UnfollowUserHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<ActionSuccessResponse> Handle(UnfollowUserCommand request,
            CancellationToken cancellationToken)
        {
            await _followerService.UnfollowUserAsync(request.UserId, request.UserToUnfollowId);

            return new ActionSuccessResponse {Message = "OK"};
        }
    }
}