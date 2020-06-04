using System.ComponentModel.DataAnnotations;

namespace Violetum.IdentityServer.Models
{
    public class LoginViewModel
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}