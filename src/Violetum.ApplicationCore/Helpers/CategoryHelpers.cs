using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public static class CategoryHelpers
    {
        public static bool UserOwnsCategory(string userId, string categoryAuthorId)
        {
            return userId == categoryAuthorId;
        }

        public static IConfigurationProvider GetCategoryMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryViewModel>();

                cfg.CreateMap<User, UserBaseViewModel>();
            });
        }
    }
}