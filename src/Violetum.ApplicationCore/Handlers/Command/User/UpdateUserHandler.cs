using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Responses;
using Violetum.ApplicationCore.ViewModels.User;

namespace Violetum.ApplicationCore.Handlers.Command.User
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