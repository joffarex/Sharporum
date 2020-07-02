using MediatR;
using Violetum.ApplicationCore.Dtos.Community;

namespace Violetum.ApplicationCore.Commands.Community
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