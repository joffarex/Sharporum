using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Post;

namespace Sharporum.Core.Handlers.Query.Post
{
    public class GetPostEntityHandler : IRequestHandler<GetPostEntityQuery, Domain.Entities.Post>
    {
        private readonly IPostService _postService;

        public GetPostEntityHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<Domain.Entities.Post> Handle(GetPostEntityQuery request, CancellationToken cancellationToken)
        {
            return await _postService.GetPostEntityAsync(request.PostId);
        }
    }
}