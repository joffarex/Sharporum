using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Violetum.Domain.Entities;

namespace Violetum.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var user = new User
                {
                    UserName = "joffarex",
                    Email = "joffarex@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Tornike",
                    LastName = "Goshadze",
                    Gender = "Male",
                    BirthDate = "2001-01-26",
                };
                userManager.CreateAsync(user, "password").GetAwaiter().GetResult();

                RunMigrations(scope);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
        }

        private static void RunMigrations(IServiceScope scope)
        {
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                .Database.Migrate();

            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Database.Migrate();

            var hostBuilder = new WebHostBuilder();
            string environment = hostBuilder.GetSetting("environment");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environment}.json")
                .Build();

            if (!context.Clients.Any())
            {
                foreach (Client client in Config.Clients(configuration))
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (IdentityResource resource in Config.Ids)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (ApiResource resource in Config.Apis)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
        }
    }
}