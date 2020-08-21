using System.Collections.Generic;
using MediatR;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Queries.User
{
    public class GetPostRanksQuery : IRequest<IEnumerable<Ranks>>
    {
    }
}