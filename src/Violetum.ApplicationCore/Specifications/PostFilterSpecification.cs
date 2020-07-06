using System.Linq;
using Joffarex.Specification;
using Joffarex.Specification.Extensions.Include;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Specifications
{
    public class PostFilterSpecification : BaseSpecification<Post>
    {
        public PostFilterSpecification(PostSearchParams searchParams)
            : base(x =>
                (string.IsNullOrEmpty(searchParams.CommunityName) || x.Community.Name == searchParams.CommunityName) &&
                (string.IsNullOrEmpty(searchParams.PostTitle) || x.Title.Contains(searchParams.PostTitle)) &&
                (string.IsNullOrEmpty(searchParams.UserId) || x.AuthorId == searchParams.UserId) &&
                (searchParams.Followers == null || searchParams.Followers.Contains(x.AuthorId))
            )
        {
            AddIncludes(query =>
                query.Include(x => x.Community).Include(x => x.Author).Include(x => x.PostVotes)
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