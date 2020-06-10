using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Violetum.ApplicationCore.Helpers
{
    public static class ProfileHelpers
    {
        public static string GetClaimByType(IEnumerable<Claim> claims, string type)
        {
            return claims.Where(x => x.Type == type).Select(x => x.Value).FirstOrDefault();
        }
    }
}