namespace Violetum.Domain.Models.SearchParams
{
    public abstract class BaseSearchParams
    {
        public string SortBy { get; set; }
        public string OrderByDir { get; set; }

        public int CurrentPage { get; set; } = 1;
        public virtual int Limit { get; } = 20;
        public int Offset => Limit * (CurrentPage - 1);
    }
}