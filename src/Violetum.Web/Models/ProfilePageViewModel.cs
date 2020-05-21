using System.Collections.Generic;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.ViewModels;

namespace Violetum.Web.Models
{
    public class ProfilePageViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
        public bool IsAuthenticatedUserFollower { get; set; }
        public FollowerDto FollowerDto { get; set; }
    }
}