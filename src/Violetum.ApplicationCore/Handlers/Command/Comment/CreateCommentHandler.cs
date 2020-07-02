using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Comment;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.ApplicationCore.Handlers.Command.Comment
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, CreatedResponse>
    {
        private readonly ICommentService _commentService;

        public CreateCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<CreatedResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            string commentId = await _commentService.CreateCommentAsync(request.UserId, request.CreateCommentDto);
            return new CreatedResponse {Id = commentId};
        }
    }
}