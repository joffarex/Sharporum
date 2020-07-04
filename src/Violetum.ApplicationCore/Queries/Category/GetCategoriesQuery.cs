using MediatR;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Category
{
    public class GetCategoriesQuery : IRequest<FilteredDataViewModel<CategoryViewModel>>
    {
        public GetCategoriesQuery(CategorySearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public CategorySearchParams SearchParams { get; set; }
    }
}