using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Category;
using Violetum.ApplicationCore.ViewModels.Category;

namespace Violetum.ApplicationCore.Handlers.Query.Category
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, GetManyResponse<CategoryViewModel>>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<GetManyResponse<CategoryViewModel>> Handle(GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CategoryViewModel> categories = await _categoryService.GetCategoriesAsync(request.SearchParams);
            int categoriesCount = await _categoryService.GetCategoriesCountAsync(request.SearchParams);

            return new GetManyResponse<CategoryViewModel>
            {
                Data = categories,
                Count = categoriesCount,
                Params = new Params
                    {Limit = request.SearchParams.Limit, CurrentPage = request.SearchParams.CurrentPage},
            };
        }
    }
}