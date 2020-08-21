using AutoMapper;
using Sharporum.Core.ViewModels.Follower;
using Sharporum.Domain.Entities;

namespace Sharporum.API.Profiles
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