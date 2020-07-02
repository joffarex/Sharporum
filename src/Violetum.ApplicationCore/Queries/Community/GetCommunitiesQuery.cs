using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Community
{
    public class GetCommunitiesQuery : IRequest<GetManyResponse<CommunityViewModel>>
    {
        public GetCommunitiesQuery(CommunitySearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public CommunitySearchParams SearchParams { get; set; }
    }
}