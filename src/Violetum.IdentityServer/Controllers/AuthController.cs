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

            User existingUserWithUsername = await _userManager.FindByNameAsync(registerViewModel.Username);

            if (existingUserWithUsername != null)
            {
                ModelState.AddModelError("UsernameExists", "User with provided username already exists");
                return View(registerViewModel);
            }

            User existingUserWithEmail = await _userManager.FindByEmailAsync(registerViewModel.Email);

            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("EmailExists", "User with provided email already exists");
                return View(registerViewModel);
            }

            var user = new User {UserName = registerViewModel.Username, Email = registerViewModel.Email};
            IdentityResult result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", new {registerViewModel.ReturnUrl});
            }

            ModelState.AddModelError("Error", "User creation failed");
            return View(registerViewModel);
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