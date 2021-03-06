﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Sharporum.API.Filters;
using Sharporum.API.Settings;

namespace Sharporum.API.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var swaggerSettings = new SwaggerSettings();
            configuration.GetSection(nameof(SwaggerSettings)).Bind(swaggerSettings);
            services.AddSingleton(swaggerSettings);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Sharporum API", Version = "v1"});

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                new Uri(
                                    $"{swaggerSettings.IdentityServer}/connect/authorize",
                                    UriKind.Absolute),
                            TokenUrl = new Uri($"{swaggerSettings.IdentityServer}/connect/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                {Constants.ApiName, "API to use issued tokens"},
                            },
                        },
                    },
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"},
                        },
                        new[] {Constants.ApiName}
                    },
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }
    }
}