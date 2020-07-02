using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Handlers.Command.Community
{
    public class UpdateCommunityImageHandler : IRequestHandler<UpdateCommunityImageCommand, CommunityResponse>
    {
        private readonly IBlobService _blobService;
        private readonly ICommunityService _communityService;

        public UpdateCommunityImageHandler(ICommunityService communityService, IBlobService blobService)
        {
            _communityService = communityService;
            _blobService = blobService;
        }

        public async Task<CommunityResponse> Handle(UpdateCommunityImageCommand request,
            CancellationToken cancellationToken)
        {
            FileData data =
                BaseHelpers.GetFileData<Domain.Entities.Community>(request.UpdateCommunityImageDto.Image,
                    request.Community.Id);
            await _blobService.UploadImageBlobAsync(data.Content, data.FileName);
            request.UpdateCommunityImageDto.Image = data.FileName;

            CommunityViewModel communityViewModel =
                await _communityService.UpdateCommunityImageAsync(request.Community, request.UpdateCommunityImageDto);

            return new CommunityResponse {Community = communityViewModel};
        }
    }
}