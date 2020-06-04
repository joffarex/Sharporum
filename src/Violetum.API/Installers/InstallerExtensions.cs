using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Violetum.API.Infrastructure;
using Violetum.Domain.Infrastructure;

namespace Violetum.API.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            List<IInstaller> installers = typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>()
                .ToList();

            foreach (IInstaller installer in installers)
            {
                installer.InstallServices(services, configuration, environment);
            }
        }

        public static void InjectCustomServicesByAttribute<TAttribute>(this IServiceCollection @this)
            where TAttribute : Attribute
        {
            Type serviceType = typeof(TAttribute);
            IEnumerable<TypeInfo> definedType = serviceType.Assembly.DefinedTypes;

            IEnumerable<TypeInfo> services = definedType
                .Where(x => x.GetTypeInfo().GetCustomAttribute<TAttribute>() != null);

            @this.AddTransient<IIdentityManager, IdentityManager>();

            foreach (TypeInfo service in services)
            {
                @this.AddTransient(service.ImplementedInterfaces.FirstOrDefault(x => !x.Name.Contains("Base")),
                    service);
            }
        }
    }
}