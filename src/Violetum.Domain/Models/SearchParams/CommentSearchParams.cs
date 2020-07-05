namespace Violetum.Domain.Models.SearchParams
{
    public class CommentSearchParams : BaseSearchParams
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}