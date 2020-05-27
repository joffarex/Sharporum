using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.AspNetCore;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Violetum.Domain.Entities;
using Violetum.Infrastructure;
using Violetum.Web.Middlewares;

namespace Violetum.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentityCore<User>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async x =>
                        {
                            // since our cookie lifetime is based on the access token one, check if we're more than halfway of the cookie lifetime
                            DateTimeOffset now = DateTimeOffset.UtcNow;
                            TimeSpan timeElapsed = now.Subtract(x.Properties.IssuedUtc.Value);
                            TimeSpan timeRemaining = x.Properties.ExpiresUtc.Value.Subtract(now);

                            if (timeElapsed > timeRemaining)
                            {
                                var identity = (ClaimsIdentity) x.Principal.Identity;
                                Claim accessTokenClaim = identity.FindFirst("access_token");
                                Claim refreshTokenClaim = identity.FindFirst("refresh_token");

                                // if we have to refresh, grab the refresh token from the claims, and request
                                // new access token and refresh token
                                string refreshToken = refreshTokenClaim.Value;
                                TokenResponse response = await new HttpClient().RequestRefreshTokenAsync(
                                    new RefreshTokenRequest
                                    {
                                        Address = "http://localhost:5000/connect/token",
                                        ClientId = "Violetum.Web",
                                        ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
                                        RefreshToken = refreshToken,
                                    });

                                if (!response.IsError)
                                {
                                    // everything went right, remove old tokens and add new ones
                                    identity.RemoveClaim(accessTokenClaim);
                                    identity.RemoveClaim(refreshTokenClaim);

                                    identity.AddClaims(new[]
                                    {
                                        new Claim("access_token", response.AccessToken),
                                        new Claim("refresh_token", response.RefreshToken),
                                    });

                                    x.ShouldRenew = true;
                                }
                            }
                        },
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "Violetum.Web";
                    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                    options.ResponseType = OidcConstants.ResponseTypes.Code;
                    options.SignedOutCallbackPath = "/Home/Index";
                    options.SaveTokens = true;
                    // TODO: possibly open up vote buttons for unauthenticated users and then redirect back to that action
                    //options.ResponseMode = OidcConstants.ResponseModes.FormPost;

                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    // claimType is basically a name what we have defined in Identity resources
                    options.ClaimActions.MapAll();
                    options.ClaimActions.DeleteClaim("amr");
                    options.ClaimActions.DeleteClaim("s_hash");
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("offline_access");

                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = x =>
                        {
                            // store both access and refresh token in the claims - hence in the cookie
                            var identity = (ClaimsIdentity) x.Principal.Identity;
                            identity.AddClaims(new[]
                            {
                                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                                new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken),
                            });

                            // so that we don't issue a session cookie but one with a fixed expiration
                            x.Properties.IsPersistent = true;

                            // align expiration of the cookie with expiration of the access token
                            var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.AccessToken);
                            x.Properties.ExpiresUtc = accessToken.ValidTo;

                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddHttpClient();

            services.AddControllersWithViews().AddFluentValidation(fv =>
            {
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddApplicationServices();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Violetum", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusCodePagesWithRedirects("/Error/{0}");
            app.ConfigureCustomExceptionMiddleware();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Violetum V1"); });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}