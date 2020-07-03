using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.User;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetCommentRanksHandler : IRequestHandler<GetCommentRanksQuery, UserRanksResponse>
    {
        private readonly IUserService _userService;

        public GetCommentRanksHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<UserRanksResponse> Handle(GetCommentRanksQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new UserRanksResponse {Ranks = _userService.GetCommentRanks()});
        }
    }
}