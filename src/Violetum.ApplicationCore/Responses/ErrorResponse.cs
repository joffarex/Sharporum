using System.Collections.Generic;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Responses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}