using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.User;

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