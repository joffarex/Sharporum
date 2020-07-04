using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Category;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Category
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, string>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.CreateCategoryAsync(request.CreateCategoryDto);
        }
    }
}