using MediatR;
using Sharporum.Core.ViewModels.Community;

namespace Sharporum.Core.Queries.Community
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