using MediatR;
using Sharporum.Core.Dtos.Community;
using Sharporum.Core.ViewModels.Community;

namespace Sharporum.Core.Commands.Community
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