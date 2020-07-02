using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Handlers.Command.Community
{
    public class UpdateCommunityHandler : IRequestHandler<UpdateCommunityCommand, CommunityResponse>
    {
        private readonly ICommunityService _communityService;

        public UpdateCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<CommunityResponse> Handle(UpdateCommunityCommand request, CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel =
                await _communityService.UpdateCommunityAsync(request.Community, request.UpdateCommunityDto);

            return new CommunityResponse {Community = communityViewModel};
        }
    }
}