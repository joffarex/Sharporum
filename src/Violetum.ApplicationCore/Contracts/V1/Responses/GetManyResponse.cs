using System.Collections.Generic;

namespace Violetum.ApplicationCore.Contracts.V1.Responses
{
    public class GetManyResponse<TViewModel>
    {
        public IEnumerable<TViewModel> Data { get; set; }
        public int Count { get; set; }
        public Params Params { get; set; }
    }
}