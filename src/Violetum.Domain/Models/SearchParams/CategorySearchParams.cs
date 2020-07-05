using Violetum.Domain.Entities;

namespace Violetum.Domain.Models.SearchParams
{
    public class CategorySearchParams : BaseSearchParams<Category>
    {
        public string CategoryName { get; set; }
    }
}