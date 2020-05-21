using System.Collections.Generic;

namespace Violetum.ApplicationCore.ViewModels
{
    public class UserFollowersViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<FollowerViewModel> Followers { get; set; }
    }
}