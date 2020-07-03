using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Category;
using Violetum.ApplicationCore.ViewModels.Category;

namespace Violetum.ApplicationCore.Handlers.Query.Category
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, CategoryResponse>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public Task<CategoryResponse> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            CategoryViewModel category = _categoryService.GetCategoryById(request.CategoryId);
            return Task.FromResult(new CategoryResponse {Category = category});
        }
    }
}