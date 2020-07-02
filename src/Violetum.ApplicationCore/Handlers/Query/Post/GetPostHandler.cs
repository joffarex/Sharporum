using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.Post;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Handlers.Query.Post
{
    public class GetPostHandler : IRequestHandler<GetPostQuery, PostResponse>
    {
        private readonly IPostService _postService;

        public GetPostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public Task<PostResponse> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            PostViewModel post = _postService.GetPost(request.PostId);
            return Task.FromResult(new PostResponse {Post = post});
        }
    }
}