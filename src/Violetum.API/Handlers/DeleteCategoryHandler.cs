using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.API.Commands;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.Domain.Entities;

namespace Violetum.API.Handlers
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
            Category category = _categoryService.GetCategoryEntity(request.CategoryId);
            await _categoryService.DeleteCategoryAsync(category);

            return Unit.Value;
        }
    }
}