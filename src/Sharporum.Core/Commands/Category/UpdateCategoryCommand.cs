using MediatR;
using Sharporum.Core.Dtos.Category;
using Sharporum.Core.ViewModels.Category;

namespace Sharporum.Core.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<CategoryViewModel>
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