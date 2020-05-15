using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Identity()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            string result = await GetUserClaims(accessToken);

            ViewBag.Json = JArray.Parse(result).ToString();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookie", "oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private async Task<string> GetUserClaims(string accessToken)
        {
            HttpClient apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            string content = await apiClient.GetStringAsync("http://localhost:5001/identity");

            return content;
        }

        /*
        private async Task RefreshAccessToken()
        {
            var serverClient = _httpClientFactory.CreateClient();
            DiscoveryDocumentResponse discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("http://localhost:5000/");
            
            string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = refreshToken,
                Address = discoveryDocument.TokenEndpoint,
                ClientId    = "Violetum.Web",
                ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
            });

            var authInfo = await HttpContext.AuthenticateAsync("Cookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);
        }
        */
    }
}