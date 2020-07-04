using System;
using System.Collections.Generic;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Responses
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
    }
}