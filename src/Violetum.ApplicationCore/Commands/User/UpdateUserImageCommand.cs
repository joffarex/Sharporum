using MediatR;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Responses;

namespace Violetum.ApplicationCore.Commands.User
{
    public class UpdateUserImageCommand : IRequest<UserResponse>
    {
        public UpdateUserImageCommand(string userId, UpdateUserImageDto updateUserImageDto)
        {
            UserId = userId;
            UpdateUserImageDto = updateUserImageDto;
        }

        public string UserId { get; set; }
        public UpdateUserImageDto UpdateUserImageDto { get; set; }
    }
}