using System.Collections.Generic;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Queries
{
    public class GetCategoriesQuery : IRequest<GetManyResponse<CategoryViewModel>>
    {
        public GetCategoriesQuery(CategorySearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public CategorySearchParams SearchParams { get; set; }
    }
}