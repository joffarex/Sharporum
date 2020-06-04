using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Helpers
{
    public static class CategoryHelpers
    {
        public static bool WhereConditionPredicate(CategorySearchParams searchParams, Category c)
        {
            var predicate = true;

            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                predicate = predicate && c.Name.Contains(searchParams.CategoryName);
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                predicate = predicate && (c.AuthorId == searchParams.UserId);
            }

            return predicate;
        }

        public static bool UserOwnsCategory(string userId, string categoryAuthorId)
        {
            return userId == categoryAuthorId;
        }
    }
}