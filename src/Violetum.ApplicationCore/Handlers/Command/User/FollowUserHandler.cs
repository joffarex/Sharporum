using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.ApplicationCore.Handlers.Command.User
{
    public class FollowUserHandler : IRequestHandler<FollowUserCommand, ActionSuccessResponse>
    {
        private readonly IFollowerService _followerService;

        public FollowUserHandler(IFollowerService followerService)
        {
            _followerService = followerService;
        }

        public async Task<ActionSuccessResponse> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            await _followerService.FollowUserAsync(request.UserId, request.UserToFollowId);

            return new ActionSuccessResponse {Message = "OK"};
        }
    }
}