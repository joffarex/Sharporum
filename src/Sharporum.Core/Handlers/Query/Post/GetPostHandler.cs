using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Post;
using Sharporum.Core.ViewModels.Post;

namespace Sharporum.Core.Handlers.Query.Post
{
    public class GetPostHandler : IRequestHandler<GetPostQuery, PostViewModel>
    {
        private readonly IPostService _postService;

        public GetPostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<PostViewModel> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            return await _postService.GetPostByIdAsync(request.PostId);
        }
    }
}