using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Queries.User
{
    public class GetUserQuery : IRequest<UserResponse>
    {
        public GetUserQuery(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}