using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Post;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Post
{
    public class DeletePostHandler : IRequestHandler<DeletePostCommand>
    {
        private readonly IPostService _postService;

        public DeletePostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<Unit> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            await _postService.DeletePostAsync(request.Post);

            return Unit.Value;
        }
    }
}