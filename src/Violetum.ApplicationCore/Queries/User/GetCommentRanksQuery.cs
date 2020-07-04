using System.Collections.Generic;
using MediatR;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Queries.User
{
    public class GetCommentRanksQuery : IRequest<IEnumerable<Ranks>>
    {
    }
}