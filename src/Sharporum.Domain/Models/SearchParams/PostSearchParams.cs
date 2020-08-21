using System.Collections.Generic;

namespace Sharporum.Domain.Models.SearchParams
{
    public class PostSearchParams : BaseSearchParams
    {
        public string UserId { get; set; }
        public string CommunityName { get; set; }
        public string PostTitle { get; set; }

        public IEnumerable<string> Followers { get; set; }
    }
}