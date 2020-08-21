using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.User;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Handlers.Query.User
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
            return await _userService.GetPostRanksAsync();
        }
    }
}