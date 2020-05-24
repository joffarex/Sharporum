using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface IUserValidators
    {
        Task<User> GetReturnedUserOrThrow(string userId);
    }
}