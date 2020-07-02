using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Queries.User
{
    public class GetCommentRanksQuery : IRequest<UserRanksResponse>
    {
    }
}