using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.User;
using Sharporum.Core.Responses;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Handlers.Query.User
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserResponse>
    {
        private readonly IUserService _userService;

        public GetUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            UserViewModel user = await _userService.GetUserAsync(request.UserId);
            IEnumerable<UserRank> userRanks = await _userService.GetUserRanksAsync(request.UserId);

            return new UserResponse {User = user, Ranks = userRanks};
        }
    }
}