using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.User;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Responses;
using Sharporum.Core.ViewModels.User;

namespace Sharporum.Core.Handlers.Command.User
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResponse>
    {
        private readonly IUserService _userService;

        public UpdateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            UserViewModel user = await _userService.UpdateUserAsync(request.UserId, request.UpdateUserDto);

            return new UserResponse {User = user};
        }
    }
}