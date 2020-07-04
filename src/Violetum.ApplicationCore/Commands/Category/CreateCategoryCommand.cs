using MediatR;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.ApplicationCore.Commands.Category
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