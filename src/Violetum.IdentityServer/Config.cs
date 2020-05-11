using System.Collections.Generic;
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
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("Violetum.API", "Violetum API"),
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "Violetum.Web",
                    ClientSecrets = {new Secret("secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = {"http://localhost:5002/signin-oidc"},

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Violetum.API",
                    },

                    AllowOfflineAccess = true,
                },
            };
    }
}