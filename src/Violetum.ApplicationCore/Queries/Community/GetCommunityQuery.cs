using MediatR;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Queries.Community
{
    public class GetCommunityQuery : IRequest<CommunityViewModel>
    {
        public GetCommunityQuery(string communityId)
        {
            CommunityId = communityId;
        }

        public string CommunityId { get; set; }
    }
}