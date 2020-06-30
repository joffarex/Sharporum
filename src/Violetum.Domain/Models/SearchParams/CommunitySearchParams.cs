namespace Violetum.Domain.Models.SearchParams
{
    public class CommunitySearchParams : BaseSearchParams
    {
        public string UserId { get; set; }
        public string CommunityName { get; set; }
    }
}