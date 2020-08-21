using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Post;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Post
{
    public class CreatePostHandler : IRequestHandler<CreatePostCommand, string>
    {
        private readonly IPostService _postService;

        public CreatePostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<string> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            return await _postService.CreatePostAsync(request.UserId, request.CreatePostDto);
        }
    }
}