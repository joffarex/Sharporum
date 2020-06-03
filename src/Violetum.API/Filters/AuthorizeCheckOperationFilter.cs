using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Violetum.API.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            IEnumerable<string> requiredScopes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Distinct();

            IEnumerable<string> enumerable = requiredScopes as string[] ?? requiredScopes.ToArray();
            if (enumerable.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
                operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"},
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [oAuthScheme] = enumerable.ToList(),
                    },
                };
            }
        }
    }
}