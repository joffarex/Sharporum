using System.Threading.Tasks;
using Violetum.Domain.Models;

namespace Violetum.Domain.Infrastructure
{
    public interface ITokenManager
    {
        Task<string> GetUserIdFromAccessToken();
        Task<UserTokens> GetUserTokens();
        Task RefreshAccessToken();
    }
}