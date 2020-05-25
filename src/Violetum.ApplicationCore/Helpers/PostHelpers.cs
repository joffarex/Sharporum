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
    }
}