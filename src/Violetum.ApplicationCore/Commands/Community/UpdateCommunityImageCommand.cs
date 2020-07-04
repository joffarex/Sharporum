using MediatR;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Commands.Community
{
    public class UpdateCommunityImageCommand : IRequest<CommunityViewModel>
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