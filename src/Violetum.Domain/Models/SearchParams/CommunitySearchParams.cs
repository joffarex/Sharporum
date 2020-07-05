using Violetum.Domain.Entities;

namespace Violetum.Domain.Models.SearchParams
{
    public class CommunitySearchParams : BaseSearchParams<Community>
    {
        public string UserId { get; set; }
        public string CommunityName { get; set; }
        public string CategoryName { get; set; }
    }
}