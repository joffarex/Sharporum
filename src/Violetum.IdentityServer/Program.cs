using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Violetum.Domain.Models;

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
                };
                userManager.CreateAsync(user, "password").GetAwaiter().GetResult();

                userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim(JwtClaimTypes.Name, "Tornike Goshadze"),
                    new Claim(JwtClaimTypes.GivenName, "Tornike"),
                    new Claim(JwtClaimTypes.FamilyName, "Goshadze"),
                    new Claim(JwtClaimTypes.Picture,
                        "https://avatars1.githubusercontent.com/u/41587092?s=460&u=bfed09c2f733b89c07e85739b6bdaaaaa6288149&v=4"),
                    new Claim(JwtClaimTypes.Gender, "male"),
                    new Claim(JwtClaimTypes.BirthDate, "2001-01-26"),
                    // new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean), // TODO: implement
                    new Claim(JwtClaimTypes.WebSite, "https://joffarex.com"),
                }).GetAwaiter().GetResult();

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

            if (!context.Clients.Any())
            {
                foreach (Client client in Config.Clients)
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