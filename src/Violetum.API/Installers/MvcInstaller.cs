using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Violetum.API.Authorization.Category;
using Violetum.API.Authorization.Category.Handlers;
using Violetum.API.Authorization.Category.Requirements;

namespace Violetum.API.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.AddHttpContextAccessor();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;


            string filePath = Path.Combine(environment.ContentRootPath, "../../cert.pfx");
            var certificate = new X509Certificate2(filePath, "password");

            var optionsTokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new X509SecurityKey(certificate),
                IssuerSigningKeyResolver =
                    (token, securityToken, kid, validationParameters) =>
                        new List<X509SecurityKey> {new X509SecurityKey(certificate)},
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "Violetum.API";
                    options.SaveToken = true;
                    options.TokenValidationParameters = optionsTokenValidationParameters;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.UpdateCategoryRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdateCategoryAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeleteCategoryRolePolicy,
                    policy => { policy.AddRequirements(new CanDeleteCategoryAuthorizationRequirement()); });
            });

            services.AddScoped<IAuthorizationHandler, CanUpdateCategoryHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeleteCategoryHandler>();

            services.AddHttpClient();

            services.AddCors(config =>
                config.AddPolicy("SPAPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

            services.AddCors();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers().AddFluentValidation(fv =>
            {
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });
        }
    }
}