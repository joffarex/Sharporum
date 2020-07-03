using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Post;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Handlers.Command.Post
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, PostResponse>
    {
        private readonly IPostService _postService;

        public UpdatePostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<PostResponse> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            PostViewModel postViewModel =
                await _postService.UpdatePostAsync(request.Post, request.UpdatePostDto);

            return new PostResponse {Post = postViewModel};
        }
    }
}