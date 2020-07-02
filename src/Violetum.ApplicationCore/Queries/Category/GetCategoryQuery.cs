﻿using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Queries.Category
{
    public class GetCategoryQuery : IRequest<CategoryResponse>
    {
        public GetCategoryQuery(string categoryId)
        {
            CategoryId = categoryId;
        }

        public string CategoryId { get; set; }
    }
}