using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Violetum.ApplicationCore;
using Violetum.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection @this)
        {
            @this.InjectServicesOfSameAttribute<RepositoryAttribute>();
            @this.InjectServicesOfSameAttribute<ValidatorAttribute>();
            @this.InjectServicesOfSameAttribute<ServiceAttribute>();

            return @this;
        }

        private static void InjectServicesOfSameAttribute<TAttribute>(this IServiceCollection @this)
            where TAttribute : Attribute
        {
            Type serviceType = typeof(TAttribute);
            IEnumerable<TypeInfo> definedType = serviceType.Assembly.DefinedTypes;

            IEnumerable<TypeInfo> services = definedType
                .Where(x => x.GetTypeInfo().GetCustomAttribute<TAttribute>() != null);

            foreach (TypeInfo service in services)
            {
                @this.AddTransient(service.ImplementedInterfaces.FirstOrDefault(x => !x.Name.Contains("Base")),
                    service);
            }
        }
    }
}