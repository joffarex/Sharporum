using MediatR;

namespace Sharporum.Core.Queries.Post
{
    public class GetPostEntityQuery : IRequest<Domain.Entities.Post>
    {
        public GetPostEntityQuery(string postId)
        {
            PostId = postId;
        }

        public string PostId { get; set; }
    }
}