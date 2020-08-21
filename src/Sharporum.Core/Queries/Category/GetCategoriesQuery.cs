using MediatR;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Category;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Queries.Category
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