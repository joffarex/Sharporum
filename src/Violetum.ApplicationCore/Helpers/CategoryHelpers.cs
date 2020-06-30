﻿using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
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