using MediatR;
using Violetum.ApplicationCore.Dtos.Community;

namespace Violetum.ApplicationCore.Commands.Community
{
    public class CreateCommunityCommand : IRequest<string>
    {
        public CreateCommunityCommand(string userId, CreateCommunityDto createCommunityDto)
        {
            UserId = userId;
            CreateCommunityDto = createCommunityDto;
        }

        public string UserId { get; set; }
        public CreateCommunityDto CreateCommunityDto { get; set; }
    }
}