using AutoMapper;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Category, PostCategoryViewModel>();
            CreateMap<CategoryDto, Category>();
        }
    }
}