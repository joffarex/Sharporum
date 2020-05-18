namespace Violetum.Domain.Entities
{
    public class Paginator
    {
        public int CurrentPage { get; set; } = 1;
        public int Limit { get; set; } = 50;

        public int Offset => Limit * (CurrentPage - 1);
    }
}