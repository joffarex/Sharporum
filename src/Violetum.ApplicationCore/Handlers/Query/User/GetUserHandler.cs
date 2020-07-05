using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.User;
using Violetum.ApplicationCore.Responses;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Handlers.Query.User
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
            IEnumerable<UserRank> userRanks = await _userService.GetUserRanks(request.UserId);

            return new UserResponse {User = user, Ranks = userRanks};
        }
    }
}