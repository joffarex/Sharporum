using System.Collections.Generic;

namespace Violetum.ApplicationCore.Contracts.V1.Responses
{
    public class QueryStringErrorResponse
    {
        public List<QueryStringErrorModel> Errors { get; set; } = new List<QueryStringErrorModel>();
    }
}