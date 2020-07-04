using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Handlers.Query.User
{
    public class GetCommentRanksHandler : IRequestHandler<GetCommentRanksQuery, IEnumerable<Ranks>>
    {
        private readonly IUserService _userService;

        public GetCommentRanksHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<IEnumerable<Ranks>> Handle(GetCommentRanksQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userService.GetCommentRanks());
        }
    }
}