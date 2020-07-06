using Joffarex.Specification;
using Joffarex.Specification.Extensions.Include;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Specifications
{
    public class CommentFilterSpecification : BaseSpecification<Comment>
    {
        public CommentFilterSpecification(CommentSearchParams searchParams)
            : base(x =>
                (string.IsNullOrEmpty(searchParams.UserId) || x.AuthorId == searchParams.UserId) &&
                (string.IsNullOrEmpty(searchParams.PostId) || x.PostId == searchParams.PostId)
            )
        {
            AddIncludes(query =>
                query.Include(x => x.Author).Include(x => x.CommentVotes)
            );

            if (searchParams.OrderByDir.ToUpper() == "DESC")
            {
                ApplyFieldOrderByDescending(searchParams.SortBy);
            }
            else
            {
                ApplyFieldOrderBy(searchParams.SortBy);
            }

            ApplyPaging(searchParams.Offset, searchParams.Limit);
        }
    }
}