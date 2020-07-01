using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.API.Commands
{
    public class CreateCategoryCommand : IRequest<CreatedResponse>
    {
        public CreateCategoryCommand(CreateCategoryDto createCategoryDto)
        {
            CreateCategoryDto = createCategoryDto;
        }

        public CreateCategoryDto CreateCategoryDto { get; set; }
    }
}