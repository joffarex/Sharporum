using AutoMapper;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
{
    public class CommunityProfile : Profile
    {
        public CommunityProfile()
        {
            CreateMap<Community, CommunityViewModel>();
            CreateMap<Community, PostCommunityViewModel>();
            CreateMap<CreateCommunityDto, Community>();
        }
    }
}