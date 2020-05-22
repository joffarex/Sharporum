namespace Violetum.Domain.Entities
{
    public class SearchParams
    {
        public string UserId { get; set; }
        public string CategoryName { get; set; }
        public string PostId { get; set; }
        public string SortBy { get; set; }
        public string OrderByDir { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int Limit { get; } = 2;
        public int Offset => Limit * (CurrentPage - 1);
    }
}