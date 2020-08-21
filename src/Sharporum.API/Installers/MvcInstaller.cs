using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Azure.Storage.Blobs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Sharporum.API.Authorization;
using Sharporum.API.Authorization.Category.Handlers;
using Sharporum.API.Authorization.Category.Requirements;
using Sharporum.API.Authorization.Comment.Handlers;
using Sharporum.API.Authorization.Comment.Requirements;
using Sharporum.API.Authorization.Community.Handlers;
using Sharporum.API.Authorization.Community.Requirements;
using Sharporum.API.Authorization.Post.Handlers;
using Sharporum.API.Authorization.Post.Requirements;
using Sharporum.API.Filters;
using Sharporum.API.Settings;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Services;
using Sharporum.Domain.Models;

namespace Sharporum.API.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddHttpContextAccessor();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            var certificateSettings = new CertificateSettings();
            configuration.GetSection(nameof(CertificateSettings)).Bind(certificateSettings);

            string filePath = Path.Combine(environment.ContentRootPath, certificateSettings.Path);
            Console.WriteLine(filePath);
            var certificate = new X509Certificate2(filePath, certificateSettings.Password);

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

            var urlSettings = new UrlSettings();
            configuration.GetSection(nameof(UrlSettings)).Bind(urlSettings);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = urlSettings.IdentityServer;
                    options.RequireHttpsMetadata = false;

                    options.Audience = Constants.ApiName;
                    options.SaveToken = true;
                    options.TokenValidationParameters = optionsTokenValidationParameters;

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            var errorDetails = new ErrorDetails
                            {
                                StatusCode = (int) HttpStatusCode.Unauthorized,
                                Message = "Unauthorized User",
                            };

                            context.Response.StatusCode = errorDetails.StatusCode;

                            // Emit the WWW-Authenticate header.
                            context.Response.Headers.Append(HeaderNames.WWWAuthenticate, context.Options.Challenge);

                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDetails));

                            context.HandleResponse();
                        },
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.UpdateCommunityRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdateCommunityAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeleteCommunityRolePolicy,
                    policy => { policy.AddRequirements(new CanDeleteCommunityAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.AddModeratorRolePolicy,
                    policy => { policy.AddRequirements(new CanAddModeratorAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeletePostRolePolicy,
                    policy => { policy.AddRequirements(new CanDeletePostAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.UpdatePostRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdatePostAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeleteCommentRolePolicy,
                    policy => { policy.AddRequirements(new CanDeleteCommentAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.UpdateCommentRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdateCommentAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.ManageCategoryRolePolicy,
                    policy => { policy.AddRequirements(new CanManageCategoryAuthorizationRequirement()); });
            });

            services.AddScoped<IAuthorizationHandler, CanUpdateCommunityHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeleteCommunityHandler>();
            services.AddScoped<IAuthorizationHandler, CanAddModeratorHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeletePostHandler>();
            services.AddScoped<IAuthorizationHandler, CanUpdatePostHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeleteCommentHandler>();
            services.AddScoped<IAuthorizationHandler, CanUpdateCommentHandler>();
            services.AddScoped<IAuthorizationHandler, CanManageCategoryHandler>();

            services.AddSingleton(x =>
                new BlobServiceClient(configuration.GetValue<string>("AzureBlobStorageConnectionString")));

            services.AddSingleton<IBlobService, BlobService>();

            services.AddHttpClient();

            services.AddCors(config =>
            {
                config.AddPolicy(Constants.CorsPolicy, builder =>
                {
                    builder.WithOrigins(urlSettings.Spa)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddCors();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers(options =>
                {
                    options.Filters.Add<ValidationFilter>();
                    options.Filters.Add<ExceptionHandlerFilter>();
                })
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                });
        }
    }
}