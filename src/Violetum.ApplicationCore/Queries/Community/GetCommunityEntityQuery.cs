using MediatR;

namespace Violetum.ApplicationCore.Queries.Community
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