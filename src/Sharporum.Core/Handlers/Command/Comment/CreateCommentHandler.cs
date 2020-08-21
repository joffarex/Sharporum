using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Comment;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Comment
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, string>
    {
        private readonly ICommentService _commentService;

        public CreateCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<string> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            return await _commentService.CreateCommentAsync(request.UserId, request.CreateCommentDto);
        }
    }
}