﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Violetum.API.Filters;

namespace Violetum.API.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Violetum API", Version = "v1"});

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                new Uri(
                                    "http://localhost:5000/connect/authorize",
                                    UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                {"Violetum.API", "API to use issued tokens"},
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
                        new[] {"Violetum.API"}
                    },
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }
    }
}