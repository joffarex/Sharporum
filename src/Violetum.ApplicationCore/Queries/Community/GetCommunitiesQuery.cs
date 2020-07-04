using MediatR;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Community
{
    public class GetCommunitiesQuery : IRequest<FilteredDataViewModel<CommunityViewModel>>
    {
        public GetCommunitiesQuery(CommunitySearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public CommunitySearchParams SearchParams { get; set; }
    }
}