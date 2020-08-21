using MediatR;
using Sharporum.Core.ViewModels.Post;

namespace Sharporum.Core.Queries.Post
{
    public class GetPostQuery : IRequest<PostViewModel>
    {
        public GetPostQuery(string postId)
        {
            PostId = postId;
        }

        public string PostId { get; set; }
    }
}