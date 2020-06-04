using System.Threading.Tasks;
using Violetum.Domain.Models;

namespace Violetum.Domain.Infrastructure
{
    public interface IIdentityManager
    {
        string GetUserId();
        Task<UserTokens> GetUserTokens();
        Task RefreshAccessToken();
    }
}