using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Category;
using Violetum.ApplicationCore.ViewModels.Category;

namespace Violetum.ApplicationCore.Handlers.Query.Category
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, CategoryViewModel>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<CategoryViewModel> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            return await _categoryService.GetCategoryByIdAsync(request.CategoryId);
        }
    }
}