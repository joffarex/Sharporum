using Joffarex.Specification;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Specifications
{
    public class CommunityFilterSpecification : BaseSpecification<Community>
    {
        public CommunityFilterSpecification(CommunitySearchParams searchParams)
            : base(x =>
                (string.IsNullOrEmpty(searchParams.CommunityName) || x.Name.Contains(searchParams.CommunityName)) &&
                (string.IsNullOrEmpty(searchParams.UserId) || x.AuthorId == searchParams.UserId)
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