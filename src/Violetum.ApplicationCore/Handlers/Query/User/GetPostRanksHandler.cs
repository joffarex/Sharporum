using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetPostRanksHandler : IRequestHandler<GetPostRanksQuery, IEnumerable<Ranks>>
    {
        private readonly IUserService _userService;

        public GetPostRanksHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<Ranks>> Handle(GetPostRanksQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetPostRanks();
        }
    }
}