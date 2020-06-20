using System;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Violetum.IdentityServer.Settings;

namespace Violetum.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("Violetum.API", "Violetum API", new List<string>
                {
                    JwtClaimTypes.Role,
                }),
            };

        public static IEnumerable<Client> Clients(IConfiguration configuration)
        {
            var urlSettings = new UrlSettings();
            configuration.GetSection(nameof(UrlSettings)).Bind(urlSettings);

            Log.Logger.Debug(JsonConvert.SerializeObject(urlSettings));

            return new List<Client>
            {
                new Client
                {
                    ClientId = "Violetum.Web",
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    RequireConsent = false,
                    IncludeJwtId = true,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 300,

                    RedirectUris = {$"{urlSettings.Web}/signin-oidc"},
                    PostLogoutRedirectUris = {$"{urlSettings.Web}/Home/Index"},

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                    },
                },
                new Client
                {
                    ClientId = "Violetum.SPA",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    IncludeJwtId = true,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 300,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = {urlSettings.Spa},
                    PostLogoutRedirectUris = {urlSettings.Spa},
                    AllowedCorsOrigins = {urlSettings.Spa},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "Violetum.API",
                    },
                },
                new Client
                {
                    ClientId = "Swagger",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireClientSecret = false,

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    IncludeJwtId = true,
                    AccessTokenLifetime = 300,

                    RedirectUris = {$"{urlSettings.Api}/swagger/oauth2-redirect.html"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "Violetum.API",
                    },
                },
            };
        }
    }
}