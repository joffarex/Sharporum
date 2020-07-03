using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Post;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Post
{
    public class VotePostHandler : IRequestHandler<VotePostCommand>
    {
        private readonly IPostService _postService;

        public VotePostHandler(IPostService postService)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        public async Task<Unit> Handle(VotePostCommand request, CancellationToken cancellationToken)
        {
            await _postService.VotePostAsync(request.PostId, request.UserId, request.PostVoteDto);

            return Unit.Value;
        }
    }
}