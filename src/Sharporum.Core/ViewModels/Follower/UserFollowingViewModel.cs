using System.Collections.Generic;

namespace Sharporum.Core.ViewModels.Follower
{
    public class UserFollowingViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<FollowingViewModel> Followings { get; set; }
    }
}