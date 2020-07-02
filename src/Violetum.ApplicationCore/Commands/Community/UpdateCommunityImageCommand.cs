using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Community;

namespace Violetum.ApplicationCore.Commands.Community
{
    public class UpdateCommunityImageCommand : IRequest<CommunityResponse>
    {
        public UpdateCommunityImageCommand(Domain.Entities.Community community,
            UpdateCommunityImageDto updateCommunityImageDto)
        {
            Community = community;
            UpdateCommunityImageDto = updateCommunityImageDto;
        }

        public Domain.Entities.Community Community { get; set; }
        public UpdateCommunityImageDto UpdateCommunityImageDto { get; set; }
    }
}