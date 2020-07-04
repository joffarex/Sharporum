using MediatR;

namespace Violetum.ApplicationCore.Commands.User
{
    public class UnfollowUserCommand : IRequest
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