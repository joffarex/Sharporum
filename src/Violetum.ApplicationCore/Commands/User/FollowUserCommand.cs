using MediatR;

namespace Violetum.ApplicationCore.Commands.User
{
    public class FollowUserCommand : IRequest
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