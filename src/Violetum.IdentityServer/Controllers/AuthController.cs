using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Violetum.Domain.Entities;
using Violetum.IdentityServer.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Violetum.IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager,
            IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
        }

        public IActionResult Index()
        {
            return Redirect("http://localhost:5002/");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // TODO: Validate vm

            SignInResult result =
                await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false,
                    false);

            if (result.Succeeded)
            {
                return Redirect(loginViewModel.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                // TODO: implement
            }
            else if (result.IsNotAllowed)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var user = new User(registerViewModel.Username);
            IdentityResult result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                return Redirect(registerViewModel.ReturnUrl);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            LogoutRequest logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index");
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}