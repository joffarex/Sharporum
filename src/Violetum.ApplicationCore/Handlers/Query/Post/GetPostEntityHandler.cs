using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Post;

namespace Violetum.ApplicationCore.Handlers.Query.Post
{
    public class GetPostEntityHandler : IRequestHandler<GetPostEntityQuery, Domain.Entities.Post>
    {
        private readonly IPostService _postService;

        public GetPostEntityHandler(IPostService postService)
        {
            _postService = postService;
        }

        public Task<Domain.Entities.Post> Handle(GetPostEntityQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_postService.GetPostEntity(request.PostId));
        }
    }
}