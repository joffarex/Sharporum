using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;

namespace Violetum.Web.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException exception)
        {
            string result;
            // context.Response.ContentType = "application/json";
            if (exception != null)
            {
                result = new ErrorDetails {Message = exception.Message, StatusCode = (int) exception.StatusCode}
                    .ToString();
                context.Response.StatusCode = (int) exception.StatusCode;
            }
            else
            {
                result = new ErrorDetails {Message = "Runtime Error", StatusCode = (int) HttpStatusCode.BadRequest}
                    .ToString();
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }

            //log error detains
            return Task.CompletedTask;
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new ErrorDetails
                {Message = exception.Message, StatusCode = (int) HttpStatusCode.InternalServerError}.ToString();
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            //log error detains
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