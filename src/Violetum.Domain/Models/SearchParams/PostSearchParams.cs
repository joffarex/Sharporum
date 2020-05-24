namespace Violetum.Domain.Models.SearchParams
{
    public class PostSearchParams : BaseSearchParams
    {
        public string UserId { get; set; }
        public string CategoryName { get; set; }
    }
}