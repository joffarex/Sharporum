using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Post;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Post
{
    public class CreatePostHandler : IRequestHandler<CreatePostCommand, CreatedResponse>
    {
        private readonly IPostService _postService;

        public CreatePostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<CreatedResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            string postId = await _postService.CreatePostAsync(request.UserId, request.CreatePostDto);
            return new CreatedResponse {Id = postId};
        }
    }
}