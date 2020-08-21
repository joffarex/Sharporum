using System.Collections.Generic;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Responses
{
    public class UserResponse
    {
        public UserViewModel User { get; set; }

        public IEnumerable<UserRank> Ranks { get; set; }
    }
}