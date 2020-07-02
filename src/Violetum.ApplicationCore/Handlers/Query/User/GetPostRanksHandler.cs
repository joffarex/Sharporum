using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.User;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetPostRanksHandler : IRequestHandler<GetPostRanksQuery, UserRanksResponse>
    {
        private readonly IUserService _userService;

        public GetPostRanksHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<UserRanksResponse> Handle(GetPostRanksQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new UserRanksResponse {Ranks = _userService.GetPostRanks()});
        }
    }
}