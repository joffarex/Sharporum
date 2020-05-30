using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Validators
{
    [Validator]
    public class UserValidators : IUserValidators
    {
        private readonly UserManager<User> _userManager;

        public UserValidators(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        public async Task<User> GetUserByIdOrThrow(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{userId} not found");
            }

            return user;
        }
    }
}