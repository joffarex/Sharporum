using System.Collections.Generic;

namespace Violetum.ApplicationCore.ViewModels
{
    public class FilteredDataViewModel<TViewModel>
    {
        public IEnumerable<TViewModel> Data { get; set; }
        public int Count { get; set; }
    }
}