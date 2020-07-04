using MediatR;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Queries.Post
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