using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Category;
using Sharporum.Core.Interfaces;
using Sharporum.Core.ViewModels.Category;

namespace Sharporum.Core.Handlers.Command.Category
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryViewModel>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<CategoryViewModel> Handle(UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            return await _categoryService.UpdateCategoryAsync(request.CategoryId, request.UpdateCategoryDto);
        }
    }
}