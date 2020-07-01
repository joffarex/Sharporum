using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.API.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<CategoryResponse>
    {
        public UpdateCategoryCommand(string categoryId, UpdateCategoryDto updateCategoryDto)
        {
            CategoryId = categoryId;
            UpdateCategoryDto = updateCategoryDto;
        }

        public string CategoryId { get; set; }
        public UpdateCategoryDto UpdateCategoryDto { get; set; }
    }
}