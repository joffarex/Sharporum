using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.User;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Handlers.Query.User
{
    public class GetCommentRanksHandler : IRequestHandler<GetCommentRanksQuery, IEnumerable<Ranks>>
    {
        private readonly IUserService _userService;

        public GetCommentRanksHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<Ranks>> Handle(GetCommentRanksQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetCommentRanksAsync();
        }
    }
}