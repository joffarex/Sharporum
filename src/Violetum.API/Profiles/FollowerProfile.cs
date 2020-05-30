using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
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