using System.Collections.Generic;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Responses
{
    public class UserResponse
    {
        public UserViewModel User { get; set; }

        public IEnumerable<UserRank> Ranks { get; set; }
    }
}