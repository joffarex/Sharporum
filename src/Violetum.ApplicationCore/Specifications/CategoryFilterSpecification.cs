using Joffarex.Specification;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Specifications
{
    public class CategoryFilterSpecification : BaseSpecification<Category>
    {
        public CategoryFilterSpecification(CategorySearchParams searchParams)
            : base(x => string.IsNullOrEmpty(searchParams.CategoryName) || x.Name.Contains(searchParams.CategoryName))
        {
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