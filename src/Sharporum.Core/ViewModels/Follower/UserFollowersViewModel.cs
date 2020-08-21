using System.Collections.Generic;

namespace Sharporum.Core.ViewModels.Follower
{
    public class UserFollowersViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<FollowerViewModel> Followers { get; set; }
    }
}