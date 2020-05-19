using System.Collections.Generic;
using Violetum.ApplicationCore.ViewModels;

namespace Violetum.Web.Models
{
    public class ProfilePageViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public IEnumerable<PostViewModel> Posts { get; set; }
    }
}