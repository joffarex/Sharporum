using MediatR;
using Sharporum.Core.Dtos.User;
using Sharporum.Core.Responses;

namespace Sharporum.Core.Commands.User
{
    public class UpdateUserCommand : IRequest<UserResponse>
    {
        public UpdateUserCommand(string userId, UpdateUserDto updateUserDto)
        {
            UserId = userId;
            UpdateUserDto = updateUserDto;
        }

        public string UserId { get; set; }
        public UpdateUserDto UpdateUserDto { get; set; }
    }
}