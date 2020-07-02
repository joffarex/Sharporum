using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Category;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.ApplicationCore.Handlers.Command.Category
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = _categoryService.GetCategoryEntity(request.CategoryId);
            await _categoryService.DeleteCategoryAsync(category);

            return Unit.Value;
        }
    }
}