using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Queries.Community
{
    public class GetCommunityQuery : IRequest<CommunityResponse>
    {
        public GetCommunityQuery(string communityId)
        {
            CommunityId = communityId;
        }

        public string CommunityId { get; set; }
    }
}