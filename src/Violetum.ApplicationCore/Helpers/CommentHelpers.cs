using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public static class CommentHelpers
    {
        public static bool WhereConditionPredicate(string userId, string postId, Comment c)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return c.AuthorId == userId;
            }

            if (!string.IsNullOrEmpty(postId))
            {
                return c.PostId == postId;
            }

            return true;
        }
    }
}