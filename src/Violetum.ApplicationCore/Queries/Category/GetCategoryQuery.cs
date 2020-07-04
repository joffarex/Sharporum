using MediatR;
using Violetum.ApplicationCore.ViewModels.Category;

namespace Violetum.ApplicationCore.Queries.Category
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