using System.Net;
using Violetum.Domain.CustomExceptions;

namespace Ardalis.GuardClauses
{
    public static class GuardClauses
    {
        public static void NullItem<TEntity>(this IGuardClause guardClause, TEntity item, string parameterName)
        {
            if (item == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{parameterName} not found");
            }
        }

        public static void NotEqual(this IGuardClause guardClause, string leftId, string rightId)
        {
            if (leftId != rightId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "This operation is not allowed");
            }
        }
    }
}