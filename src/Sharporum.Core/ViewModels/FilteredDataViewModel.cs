using System.Collections.Generic;

namespace Sharporum.Core.ViewModels
{
    public class FilteredDataViewModel<TViewModel>
    {
        public IEnumerable<TViewModel> Data { get; set; }
        public int Count { get; set; }
    }
}