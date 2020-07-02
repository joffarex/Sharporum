using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Category;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Category;

namespace Violetum.ApplicationCore.Handlers.Command.Category
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = _categoryService.GetCategoryEntity(request.CategoryId);

            CategoryViewModel categoryViewModel =
                await _categoryService.UpdateCategoryAsync(category, request.UpdateCategoryDto);

            return new CategoryResponse {Category = categoryViewModel};
        }
    }
}