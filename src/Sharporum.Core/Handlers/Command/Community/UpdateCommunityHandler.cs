using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Community;
using Sharporum.Core.Interfaces;
using Sharporum.Core.ViewModels.Community;

namespace Sharporum.Core.Handlers.Command.Community
{
    public class UpdateCommunityHandler : IRequestHandler<UpdateCommunityCommand, CommunityViewModel>
    {
        private readonly ICommunityService _communityService;

        public UpdateCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<CommunityViewModel> Handle(UpdateCommunityCommand request,
            CancellationToken cancellationToken)
        {
            return await _communityService.UpdateCommunityAsync(request.Community, request.UpdateCommunityDto);
        }
    }
}