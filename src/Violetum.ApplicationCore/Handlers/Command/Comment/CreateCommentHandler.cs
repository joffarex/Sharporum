using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Comment;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Comment
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