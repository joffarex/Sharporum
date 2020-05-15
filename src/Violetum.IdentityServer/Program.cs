using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Violetum.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = new IdentityUser("joffarex");
                userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                // added in identity token
                userManager.AddClaimAsync(user, new Claim("claim.userfield.test", "test_value")).GetAwaiter()
                    .GetResult();
                // added in access token
                userManager.AddClaimAsync(user, new Claim("claim.api.userfield.test", "test.api_value")).GetAwaiter()
                    .GetResult();
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
    }
}