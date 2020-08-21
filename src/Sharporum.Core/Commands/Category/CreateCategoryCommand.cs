using MediatR;
using Sharporum.Core.Dtos.Category;

namespace Sharporum.Core.Commands.Category
{
    public class CreateCategoryCommand : IRequest<string>
    {
        public CreateCategoryCommand(CreateCategoryDto createCategoryDto)
        {
            CreateCategoryDto = createCategoryDto;
        }

        public CreateCategoryDto CreateCategoryDto { get; set; }
    }
}