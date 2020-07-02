using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Commands.User
{
    public class FollowUserCommand : IRequest<ActionSuccessResponse>
    {
        public FollowUserCommand(string userId, string userToFollowId)
        {
            UserId = userId;
            UserToFollowId = userToFollowId;
        }

        public string UserId { get; set; }
        public string UserToFollowId { get; set; }
    }
}