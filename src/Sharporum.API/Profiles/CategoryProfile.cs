using AutoMapper;
using Sharporum.Core.Dtos.Category;
using Sharporum.Core.ViewModels.Category;
using Sharporum.Domain.Entities;

namespace Sharporum.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CreateCategoryDto, Category>();
        }
    }
}