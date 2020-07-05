using Violetum.Domain.Entities;

namespace Violetum.Domain.Models.SearchParams
{
    public class CommentSearchParams : BaseSearchParams<Comment>
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}