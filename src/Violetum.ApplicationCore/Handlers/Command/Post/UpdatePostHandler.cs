using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Handlers.Command.Post
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, PostViewModel>
    {
        private readonly IPostService _postService;

        public UpdatePostHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<PostViewModel> Handle(UpdatePostCommand request,
            CancellationToken cancellationToken)
        {
            return await _postService.UpdatePostAsync(request.Post, request.UpdatePostDto);
        }
    }
}