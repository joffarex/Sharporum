using System.Collections.Generic;

namespace Violetum.ApplicationCore.ViewModels.Follower
{
    public class UserFollowersViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<FollowerViewModel> Followers { get; set; }
    }
}