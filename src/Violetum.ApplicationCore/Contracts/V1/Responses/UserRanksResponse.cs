using System.Collections.Generic;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Contracts.V1.Responses
{
    public class UserRanksResponse
    {
        public IEnumerable<Ranks> Ranks { get; set; }
    }
}