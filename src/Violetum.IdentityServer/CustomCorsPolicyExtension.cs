using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomCorsPolicyExtension
    {
        public static IIdentityServerBuilder AddCustomCorsPolicy(this IIdentityServerBuilder builder)
        {
            var existingCors = builder.Services.Where(x => x.ServiceType == typeof(ICorsPolicyService)).LastOrDefault();
            if (existingCors != null &&
                existingCors.ImplementationType == typeof(DefaultCorsPolicyService) &&
                existingCors.Lifetime == ServiceLifetime.Transient)
            {
                builder.Services.AddTransient<ICorsPolicyService, CustomCorsPolicyService>();
            }

            return builder;
        }
    }

    public class CustomCorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(origin.Equals("http://localhost:4200"));
        }
    }
}