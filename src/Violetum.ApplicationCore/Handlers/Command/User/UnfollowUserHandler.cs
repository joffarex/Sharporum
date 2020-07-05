﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.User
{
    public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand>
    {
        private readonly IUserService _userService;

        public UnfollowUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            await _userService.UnfollowUserAsync(request.UserId, request.UserToUnfollowId);
            return Unit.Value;
        }
    }
}