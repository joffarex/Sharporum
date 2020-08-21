using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Post;
using Sharporum.Core.Helpers;
using Sharporum.Core.Interfaces;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Handlers.Command.Post
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, PostViewModel>
    {
        private readonly IBlobService _blobService;
        private readonly IPostService _postService;

        public UpdatePostHandler(IPostService postService, IBlobService blobService)
        {
            _postService = postService;
            _blobService = blobService;
        }

        public async Task<PostViewModel> Handle(UpdatePostCommand request,
            CancellationToken cancellationToken)
        {
            if (PostHelpers.IsContentFile(request.UpdatePostDto.Content) &&
                !request.Post.ContentType.Equals("application/text"))
            {
                FileData data = BaseHelpers.GetContentFileData<Domain.Entities.Post>(
                    request.UpdatePostDto.Content, request.Post.Id
                );
                request.UpdatePostDto.Content = data.FileName;
                if (request.Post.ContentType.Equals(request.UpdatePostDto.Content.GetContentType()))
                {
                    await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
                }
            }

            return await _postService.UpdatePostAsync(request.Post, request.UpdatePostDto);
        }
    }
}