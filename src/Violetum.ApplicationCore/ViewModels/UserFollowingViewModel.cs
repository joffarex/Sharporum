using System.Collections.Generic;

namespace Violetum.ApplicationCore.ViewModels
{
    public class UserFollowingViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<FollowingViewModel> Followings { get; set; }
    }
}