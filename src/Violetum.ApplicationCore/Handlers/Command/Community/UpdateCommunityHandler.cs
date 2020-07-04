using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Handlers.Command.Community
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