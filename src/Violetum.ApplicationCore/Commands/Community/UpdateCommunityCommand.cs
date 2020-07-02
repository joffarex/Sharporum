using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Community;

namespace Violetum.ApplicationCore.Commands.Community
{
    public class UpdateCommunityCommand : IRequest<CommunityResponse>
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