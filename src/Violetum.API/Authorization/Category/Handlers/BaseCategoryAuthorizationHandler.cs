using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Violetum.API.Authorization.Category.Handlers
{
    public class
        BaseCategoryAuthorizationHandler<TAuthorizationRequirement> : AuthorizationHandler<TAuthorizationRequirement>
        where TAuthorizationRequirement : IAuthorizationRequirement
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseCategoryAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            CategoryId = GetCategoryIdFromRouteData();
            RoleBase = $"{nameof(Category)}/{CategoryId}";
        }

        protected string CategoryId { get; }
        protected string RoleBase { get; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            TAuthorizationRequirement requirement)
        {
            return Task.CompletedTask;
        }

        protected string GetCategoryIdFromRouteData()
        {
            RouteData routeData = _httpContextAccessor.HttpContext.GetRouteData();

            var categoryIdParam = routeData?.Values["categoryId"]?.ToString();
            return string.IsNullOrWhiteSpace(categoryIdParam) ? string.Empty : categoryIdParam;
        }
    }
}