using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Sharporum.Domain.CustomExceptions
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, Exception inner) : this(statusCode, inner.ToString())
        {
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, JObject errorObject) : this(statusCode,
            errorObject.ToString())
        {
            ContentType = @"application/json";
        }

        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = @"text/plain";
    }
}