using System.Collections.Generic;
using System.Linq;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Helpers
{
    public class PostHelpers
    {
        public static bool WhereConditionPredicate(PostSearchParams searchParams, Post p)
        {
            var predicate = true;

            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                predicate = predicate && (p.Category.Name == searchParams.CategoryName);
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                predicate = predicate && (p.AuthorId == searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.PostTitle))
            {
                predicate = predicate && p.Title.Contains(searchParams.PostTitle);
            }

            return predicate;
        }

        public static bool WhereConditionPredicate(PostSearchParams searchParams, Post p, IEnumerable<string> followers)
        {
            return WhereConditionPredicate(searchParams, p) && followers.Contains(p.AuthorId);
        }

        public static bool UserOwnsPost(string userId, string postAuthorId)
        {
            return userId == postAuthorId;
        }
    }
}