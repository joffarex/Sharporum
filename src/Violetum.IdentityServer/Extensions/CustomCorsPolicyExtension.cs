using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using Violetum.IdentityServer.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomCorsPolicyExtension
    {
        public static IIdentityServerBuilder AddCustomCorsPolicy(this IIdentityServerBuilder builder)
        {
            ServiceDescriptor existingCors = builder.Services.Where(x => x.ServiceType == typeof(ICorsPolicyService))
                .LastOrDefault();
            if ((existingCors != null) &&
                (existingCors.ImplementationType == typeof(DefaultCorsPolicyService)) &&
                (existingCors.Lifetime == ServiceLifetime.Transient))
            {
                builder.Services.AddTransient<ICorsPolicyService, CustomCorsPolicyService>();
            }

            return builder;
        }
    }

    public class CustomCorsPolicyService : ICorsPolicyService
    {
        private readonly IConfiguration _configuration;

        public CustomCorsPolicyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var urlSettings = new UrlSettings();
            _configuration.GetSection(nameof(UrlSettings)).Bind(urlSettings);

            return Task.FromResult(origin.Equals(urlSettings.Spa));
        }
    }
}