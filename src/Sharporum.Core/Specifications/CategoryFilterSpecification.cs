using Joffarex.Specification;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Specifications
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