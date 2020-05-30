using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Violetum.ApplicationCore;
using Violetum.Infrastructure;

namespace Violetum.API.Installers
{
    public class CustomServicesInstaller : IInstaller

    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.InjectCustomServicesByAttribute<RepositoryAttribute>();
            services.InjectCustomServicesByAttribute<ValidatorAttribute>();
            services.InjectCustomServicesByAttribute<ServiceAttribute>();
        }
    }
}