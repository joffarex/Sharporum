using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.ApplicationCore.ViewModels.User;
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

        public static IConfigurationProvider GetPostMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostViewModel>()
                    .ForMember(
                        p => p.VoteCount,
                        opt => opt.MapFrom(
                            x => x.PostVotes.Sum(y => y.Direction)
                        )
                    );

                cfg.CreateMap<User, UserBaseViewModel>();
                cfg.CreateMap<Category, PostCategoryViewModel>();
            });
        }
    }
}