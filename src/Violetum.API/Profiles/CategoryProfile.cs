using AutoMapper;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();
            CreateMap<Category, PostCategoryViewModel>();
            CreateMap<CreateCategoryDto, Category>();
        }
    }
}