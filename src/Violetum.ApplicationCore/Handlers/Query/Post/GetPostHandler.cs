using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Post;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Handlers.Query.Post
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
            return await _postService.GetPost(request.PostId);
        }
    }
}