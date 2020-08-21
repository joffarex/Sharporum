using MediatR;
using Sharporum.Core.Dtos.Community;

namespace Sharporum.Core.Commands.Community
{
    public class AddModeratorCommand : IRequest
    {
        public AddModeratorCommand(Domain.Entities.Community community, AddModeratorDto addModeratorDto)
        {
            Community = community;
            AddModeratorDto = addModeratorDto;
        }

        public Domain.Entities.Community Community { get; set; }
        public AddModeratorDto AddModeratorDto { get; set; }
    }
}