using MediatR;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels.Category;

namespace Violetum.ApplicationCore.Commands.Category
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