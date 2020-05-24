namespace Violetum.Domain.Models.SearchParams
{
    public class CategorySearchParams : BaseSearchParams
    {
        public string UserId { get; set; }
        public string CategoryName { get; set; }
        public override int Limit { get; } = 20;
    }
}