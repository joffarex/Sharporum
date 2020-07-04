using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Post;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Handlers.Command.Post
{
    public class CreatePostWithFileHandler : IRequestHandler<CreatePostWithFileCommand, string>
    {
        private readonly IBlobService _blobService;
        private readonly IPostService _postService;

        public CreatePostWithFileHandler(IPostService postService, IBlobService blobService)
        {
            _postService = postService;
            _blobService = blobService;
        }

        public async Task<string> Handle(CreatePostWithFileCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Post post =
                await _postService.CreatePostWithFileAsync(request.UserId, request.CreatePostWithFileDto);

            if (!PostHelpers.IsContentFile(request.CreatePostWithFileDto.Content))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Post creation failed");
            }

            FileData data = BaseHelpers.GetContentFileData<Domain.Entities.Post>(
                request.CreatePostWithFileDto.Content, post.Id
            );

            if (post.ContentType.Equals(data.FileName.GetContentType()))
            {
                await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
                request.CreatePostWithFileDto.Content = data.FileName;
                request.CreatePostWithFileDto.ContentType = data.FileName.GetContentType();

                await _postService.UpdatePostAsync(post, new UpdatePostDto
                {
                    Content = request.CreatePostWithFileDto.Content,
                    Title = post.Title,
                });
            }
            else
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                    "Can not update content type of a post");
            }

            return post.Id;
        }
    }
}