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
                    RequireConsent = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = {"http://localhost:5002/signin-oidc"},

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {"http://localhost:5002/Home/Index"},

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                    },

                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,
                },
                new Client
                {
                    ClientId = "Violetum.SPA",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = {"http://localhost:4200"},
                    PostLogoutRedirectUris = {"http://localhost:4200"},
                    AllowedCorsOrigins = {"http://localhost:4200"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "Violetum.API",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    IncludeJwtId = true,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                },
                new Client
                {
                    ClientId = "Violetum.API.Swagger",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RequireClientSecret = false,
                    RedirectUris = {"http://localhost:5001/swagger/oauth2-redirect.html"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "Violetum.API",
                    },

                    AccessTokenLifetime = 3600,
                },
            };
    }
}