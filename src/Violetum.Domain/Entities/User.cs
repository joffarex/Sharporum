using Microsoft.AspNetCore.Identity;

namespace Violetum.Domain.Entities
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