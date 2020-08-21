using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Community;
using Sharporum.Core.Helpers;
using Sharporum.Core.Interfaces;
using Sharporum.Core.ViewModels.Community;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Handlers.Command.Community
{
    public class
        UpdateCommunityImageHandler : IRequestHandler<UpdateCommunityImageCommand, CommunityViewModel>
    {
        private readonly IBlobService _blobService;
        private readonly ICommunityService _communityService;

        public UpdateCommunityImageHandler(ICommunityService communityService, IBlobService blobService)
        {
            _communityService = communityService;
            _blobService = blobService;
        }

        public async Task<CommunityViewModel> Handle(UpdateCommunityImageCommand request,
            CancellationToken cancellationToken)
        {
            FileData data = BaseHelpers.GetContentFileData<Domain.Entities.Community>(
                request.UpdateCommunityImageDto.Image, request.Community.Id
            );
            await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
            request.UpdateCommunityImageDto.Image = data.FileName;

            return await _communityService.UpdateCommunityImageAsync(request.Community,
                request.UpdateCommunityImageDto);
        }
    }
}