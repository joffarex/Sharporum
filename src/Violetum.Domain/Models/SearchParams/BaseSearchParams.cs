namespace Violetum.Domain.Models.SearchParams
{
    public abstract class BaseSearchParams
    {
        public string SortBy { get; set; } = "CreatedAt";
        public string OrderByDir { get; set; } = "desc";

        public int CurrentPage
        {
            get => CurrentPage;
            set => CurrentPage = value < 1 ? 1 : value;
        }

        public int Limit
        {
            get => Limit;
            set => Limit = value > 100 ? 100 : value;
        }

        public int Offset => Limit * (CurrentPage - 1);
    }
}