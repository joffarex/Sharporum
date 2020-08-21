using MediatR;
using Sharporum.Core.Dtos.User;
using Sharporum.Core.Responses;

namespace Sharporum.Core.Commands.User
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