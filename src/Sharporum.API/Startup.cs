using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Sharporum.API.Installers;
using Sharporum.API.Settings;
using Sharporum.Core.Responses;

namespace Sharporum.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallServicesInAssembly(_configuration, _environment);
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(x => new HealthCheck
                        {
                            Component = x.Key,
                            Status = x.Value.Status.ToString(),
                            Description = x.Value.Description,
                        }),
                        Duration = report.TotalDuration,
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                },
            });

            app.UseSerilogRequestLogging();
            app.UseCors(Constants.CorsPolicy);
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseSwagger();

            var swaggerSettings = app.ApplicationServices.GetRequiredService<SwaggerSettings>();
            configuration.GetSection(nameof(SwaggerSettings)).Bind(swaggerSettings);

            app.UseSwaggerUI(options => { options.SwaggerEndpoint(swaggerSettings.Endpoint, swaggerSettings.Name); });
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}