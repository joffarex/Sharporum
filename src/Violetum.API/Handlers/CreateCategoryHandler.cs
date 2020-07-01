using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.API.Commands;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.API.Handlers
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CreatedResponse>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<CreatedResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            string categoryId = await _categoryService.CreateCategoryAsync(request.CreateCategoryDto);
            return new CreatedResponse {Id = categoryId};
        }
    }
}