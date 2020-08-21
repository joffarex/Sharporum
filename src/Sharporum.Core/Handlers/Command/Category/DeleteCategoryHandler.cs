using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Category;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Category
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
            await _categoryService.DeleteCategoryAsync(request.CategoryId);

            return Unit.Value;
        }
    }
}