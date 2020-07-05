using System.Collections.Generic;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Responses
{
    public class FilteredResponse<TViewModel>
    {
        public FilteredResponse(ISearchParams<TViewModel> searchParams)
        {
            Limit = searchParams.Limit;
            CurrentPage = searchParams.CurrentPage;
        }

        public IEnumerable<TViewModel> Data { get; set; }
        public int Count { get; set; }
        public int Limit { get; set; }
        public int CurrentPage { get; set; }
    }
}