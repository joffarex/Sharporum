using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Commands.User
{
    public class UnfollowUserCommand : IRequest<ActionSuccessResponse>
    {
        public UnfollowUserCommand(string userId, string userToUnfollowId)
        {
            UserId = userId;
            UserToUnfollowId = userToUnfollowId;
        }

        public string UserId { get; set; }
        public string UserToUnfollowId { get; set; }
    }
}