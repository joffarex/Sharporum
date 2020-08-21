using System.Collections.Generic;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Responses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}