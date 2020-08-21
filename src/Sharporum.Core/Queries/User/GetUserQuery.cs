using MediatR;
using Sharporum.Core.Responses;

namespace Sharporum.Core.Queries.User
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