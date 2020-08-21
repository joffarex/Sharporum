using MediatR;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Community;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Queries.Community
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