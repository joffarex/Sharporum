using System.Collections.Generic;

namespace Violetum.ApplicationCore.Responses
{
    public class FilteredResponse<TViewModel>
    {
        public IEnumerable<TViewModel> Data { get; set; }
        public int Count { get; set; }
        public int Limit { get; set; }
        public int CurrentPage { get; set; }
    }
}