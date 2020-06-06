using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Violetum.API.Authorization;
using Violetum.API.Authorization.Category.Handlers;
using Violetum.API.Authorization.Category.Requirements;
using Violetum.API.Authorization.Comment.Handlers;
using Violetum.API.Authorization.Comment.Requirements;
using Violetum.API.Authorization.Post.Handlers;
using Violetum.API.Authorization.Post.Requirements;
using Violetum.API.Filters;
using Violetum.Domain.Models;

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
                options.AddPolicy(PolicyConstants.UpdateCategoryRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdateCategoryAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeleteCategoryRolePolicy,
                    policy => { policy.AddRequirements(new CanDeleteCategoryAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeletePostRolePolicy,
                    policy => { policy.AddRequirements(new CanDeletePostAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.UpdatePostRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdatePostAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.DeleteCommentRolePolicy,
                    policy => { policy.AddRequirements(new CanDeleteCommentAuthorizationRequirement()); });
                options.AddPolicy(PolicyConstants.UpdateCommentRolePolicy,
                    policy => { policy.AddRequirements(new CanUpdateCommentAuthorizationRequirement()); });
            });

            services.AddScoped<IAuthorizationHandler, CanUpdateCategoryHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeleteCategoryHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeletePostHandler>();
            services.AddScoped<IAuthorizationHandler, CanUpdatePostHandler>();
            services.AddScoped<IAuthorizationHandler, CanDeleteCommentHandler>();
            services.AddScoped<IAuthorizationHandler, CanUpdateCommentHandler>();

            services.AddHttpClient();

            services.AddCors(config =>
            {
                config.AddPolicy("SPAPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
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
                .AddFluentValidation(fv => { fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false; });
        }
    }
}