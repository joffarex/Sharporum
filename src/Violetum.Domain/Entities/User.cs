using Microsoft.AspNetCore.Identity;

namespace Violetum.Domain.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
        }

        public User(string username)
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
    }
}