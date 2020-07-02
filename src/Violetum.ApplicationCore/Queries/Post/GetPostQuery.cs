using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Queries.Post
{
    public class GetPostQuery : IRequest<PostResponse>
    {
        public GetPostQuery(string postId)
        {
            PostId = postId;
        }

        public string PostId { get; set; }
    }
}