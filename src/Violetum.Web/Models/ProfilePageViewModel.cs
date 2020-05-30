using System.Collections.Generic;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.ApplicationCore.ViewModels.User;

namespace Violetum.Web.Models
{
    public class ProfilePageViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
        public bool IsAuthenticatedUserFollower { get; set; }
        public FollowActionDto FollowActionDto { get; set; }
    }
}