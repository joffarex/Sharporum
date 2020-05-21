using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Web.Infrastructure
{
    public class TokenManager : ITokenManager
    {
        private readonly DiscoveryDocumentResponse _discoveryDocument;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpContext _httpContext;

        public TokenManager(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContext = httpContextAccessor.HttpContext;

            // TODO: experimental
            _discoveryDocument = _httpClientFactory.CreateClient()
                .GetDiscoveryDocumentAsync("http://localhost:5000/")
                .GetAwaiter()
                .GetResult();
        }

        public async Task<string> GetUserIdFromAccessToken()
        {
            string accessToken = await _httpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            string userId = jwtToken.Subject;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Invalid token");
            }

            return userId;
        }

        public async Task<UserTokens> GetUserTokens()
        {
            string refreshToken = await _httpContext.GetTokenAsync("refresh_token");
            HttpClient refreshTokenClient = _httpClientFactory.CreateClient();

            TokenResponse tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = refreshToken,
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = "Violetum.Web",
                ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
            });

            return new UserTokens
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                IdentityToken = tokenResponse.IdentityToken,
            };
        }

        public async Task RefreshAccessToken()
        {
            UserTokens userTokens = await GetUserTokens();

            AuthenticateResult authInfo =
                await _httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            authInfo.Properties.UpdateTokenValue("access_token", userTokens.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", userTokens.RefreshToken);

            await _httpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);
        }
    }
}