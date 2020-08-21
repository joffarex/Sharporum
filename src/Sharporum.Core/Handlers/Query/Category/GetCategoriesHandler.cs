using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Category;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Category;

namespace Sharporum.Core.Handlers.Query.Category
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, FilteredDataViewModel<CategoryViewModel>>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<FilteredDataViewModel<CategoryViewModel>> Handle(GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategoriesAsync(request.SearchParams);
            int categoriesCount = await _categoryService.GetCategoriesCountAsync(request.SearchParams);

            return new FilteredDataViewModel<CategoryViewModel>
            {
                Data = categories,
                Count = categoriesCount,
            };
        }
    }
}