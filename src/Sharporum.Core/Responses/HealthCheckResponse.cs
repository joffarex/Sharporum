using System;
using System.Collections.Generic;
using Sharporum.Domain.Models;

namespace Sharporum.Core.Responses
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
    }
}