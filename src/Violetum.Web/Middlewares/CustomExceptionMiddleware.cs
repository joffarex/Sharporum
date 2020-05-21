using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;

namespace Violetum.Web.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly ILogger<CustomExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpStatusCodeException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException exception)
        {
            ErrorDetails result;
            if (exception != null)
            {
                result = new ErrorDetails {Message = exception.Message, StatusCode = (int) exception.StatusCode};
                context.Response.StatusCode = (int) exception.StatusCode;
            }
            else
            {
                result = new ErrorDetails {Message = "Runtime Error", StatusCode = (int) HttpStatusCode.BadRequest};
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }

            _logger.LogError(exception, result.Message);
            return Task.CompletedTask;
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new ErrorDetails
                {Message = exception.Message, StatusCode = (int) HttpStatusCode.InternalServerError};
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

            _logger.LogError(exception, result.Message);
            return Task.CompletedTask;
        }
    }

    public static class CustomExceptionMiddlewareExtension
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}