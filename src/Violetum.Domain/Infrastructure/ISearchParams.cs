namespace Violetum.Domain.Infrastructure
{
    public interface ISearchParams<TEntity>
    {
        public string SortBy { get; set; }
        public string OrderByDir { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int Offset => Limit * (CurrentPage - 1);
    }
}