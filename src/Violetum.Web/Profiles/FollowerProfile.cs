using AutoMapper;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class FollowerProfile : Profile
    {
        public FollowerProfile()
        {
            CreateMap<Follower, FollowerViewModel>();
            CreateMap<Follower, FollowingViewModel>();
        }
    }
}