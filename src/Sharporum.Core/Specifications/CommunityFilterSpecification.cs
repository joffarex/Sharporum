using Joffarex.Specification;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Specifications
{
    public class CommunityFilterSpecification : BaseSpecification<Community>
    {
        public CommunityFilterSpecification(CommunitySearchParams searchParams)
            : base(x =>
                (string.IsNullOrEmpty(searchParams.CommunityName) || x.Name.Contains(searchParams.CommunityName)) &&
                (string.IsNullOrEmpty(searchParams.UserId) || (x.AuthorId == searchParams.UserId))
            )
        {
            AddIncludes(query => query.Include(x => x.Author));

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