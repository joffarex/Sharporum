namespace Violetum.Domain.Entities
{
    public class Paginator
    {
        public int CurrentPage { get; set; }
        public int Limit { get; set; }

        public int Offset => Limit * (CurrentPage - 1);
    }
}