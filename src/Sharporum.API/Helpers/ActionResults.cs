using Microsoft.AspNetCore.Mvc;

namespace Sharporum.API.Helpers
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