using Microsoft.AspNetCore.Mvc;

namespace Violetum.API.Helpers
{
    public static class ActionResults
    {
        public static IActionResult UnauthorizedResult(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }
    }
}