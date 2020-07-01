using MediatR;

namespace Violetum.API.Commands.Category
{
    public class DeleteCategoryCommand : IRequest
    {
        public DeleteCategoryCommand(string categoryId)
        {
            CategoryId = categoryId;
        }

        public string CategoryId { get; set; }
    }
}