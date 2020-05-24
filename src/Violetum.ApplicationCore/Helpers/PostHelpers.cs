using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public class PostHelpers
    {
        public static bool WhereConditionPredicate(string userId, string categoryName, Post p)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(categoryName))
            {
                return (p.Category.Name == categoryName) && (p.AuthorId == userId);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                return p.Category.Name == categoryName;
            }

            if (!string.IsNullOrEmpty(userId))
            {
                return p.AuthorId == userId;
            }

            return true;
        }
    }
}