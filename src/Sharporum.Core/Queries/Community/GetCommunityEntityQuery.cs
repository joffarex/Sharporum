using MediatR;

namespace Sharporum.Core.Queries.Community
{
    public class GetCommunityEntityQuery : IRequest<Domain.Entities.Community>
    {
        public GetCommunityEntityQuery(string communityId)
        {
            CommunityId = communityId;
        }

        public string CommunityId { get; set; }
    }
}