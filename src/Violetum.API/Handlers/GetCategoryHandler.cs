using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.API.Queries;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.API.Handlers
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
            var category = _categoryService.GetCategoryById(request.CategoryId);
            return Task.FromResult(new CategoryResponse {Category = category});
        }
    }
}