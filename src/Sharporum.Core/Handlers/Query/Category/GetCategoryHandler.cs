using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Category;
using Sharporum.Core.ViewModels.Category;

namespace Sharporum.Core.Handlers.Query.Category
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