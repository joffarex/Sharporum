using MediatR;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Commands.Community
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