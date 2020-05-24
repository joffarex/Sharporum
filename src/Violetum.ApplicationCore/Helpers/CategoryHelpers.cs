using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public static class CategoryHelpers
    {
        public static bool WhereConditionPredicate(string userId, string categoryName, Category c)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(categoryName))
            {
                return c.Name.Contains(categoryName) && (c.AuthorId == userId);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                return c.Name.Contains(categoryName);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                return c.AuthorId == userId;
            }

            return true;
        }
    }
}