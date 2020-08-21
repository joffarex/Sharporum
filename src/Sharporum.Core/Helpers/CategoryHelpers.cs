using AutoMapper;
using Sharporum.Core.ViewModels.Category;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Entities;

namespace Sharporum.Core.Helpers
{
    public class CategoryHelpers
    {
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