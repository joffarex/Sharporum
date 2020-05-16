using Microsoft.AspNetCore.Identity;

namespace Violetum.Domain.Models
{
    public class User : IdentityUser
    {
        public User()
        {
        }

        public User(string username) : base(username)
        {
        }
    }
}