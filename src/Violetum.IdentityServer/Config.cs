using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

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

        public static IEnumerable<Client> Clients =>
            new List<Client>
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

                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/Home/Index"},

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

                    RedirectUris = {"http://localhost:4200"},
                    PostLogoutRedirectUris = {"http://localhost:4200"},
                    AllowedCorsOrigins = {"http://localhost:4200"},

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

                    RedirectUris = {"http://localhost:5001/swagger/oauth2-redirect.html"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "Violetum.API",
                    },
                },
            };
    }
}