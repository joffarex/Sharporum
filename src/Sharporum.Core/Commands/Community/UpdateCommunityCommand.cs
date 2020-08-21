using MediatR;
using Sharporum.Core.Dtos.Community;
using Sharporum.Core.ViewModels.Community;

namespace Sharporum.Core.Commands.Community
{
    public class UpdateCommunityCommand : IRequest<CommunityViewModel>
    {
        public UpdateCommunityCommand(string communityId, Domain.Entities.Community community,
            UpdateCommunityDto updateCommunityDto)
        {
            CommunityId = communityId;
            Community = community;
            UpdateCommunityDto = updateCommunityDto;
        }

        public string CommunityId { get; set; }
        public Domain.Entities.Community Community { get; set; }
        public UpdateCommunityDto UpdateCommunityDto { get; set; }
    }
}