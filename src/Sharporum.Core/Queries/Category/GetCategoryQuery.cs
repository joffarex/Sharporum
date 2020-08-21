using MediatR;
using Sharporum.Core.ViewModels.Category;

namespace Sharporum.Core.Queries.Category
{
    public class GetCategoryQuery : IRequest<CategoryViewModel>
    {
        public GetCategoryQuery(string categoryId)
        {
            CategoryId = categoryId;
        }

        public string CategoryId { get; set; }
    }
}