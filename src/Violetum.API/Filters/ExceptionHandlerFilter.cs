using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Models;

namespace Violetum.API.Filters
{
    public class ExceptionHandlerFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var errorDetails = new ErrorDetails();
            Exception exception = context.Exception;

            switch (exception)
            {
                case null:
                    errorDetails.StatusCode = (int) HttpStatusCode.BadRequest;
                    errorDetails.Message = "Runtime Error";
                    break;
                case HttpStatusCodeException httpStatusCodeException:
                    errorDetails.StatusCode = (int) httpStatusCodeException.StatusCode;
                    errorDetails.Message = httpStatusCodeException.Message;
                    break;
                default:
                    errorDetails.StatusCode = (int) HttpStatusCode.InternalServerError;
                    errorDetails.Message = exception.Message;
                    break;
            }

            context.Result = new ObjectResult(errorDetails)
            {
                StatusCode = errorDetails.StatusCode,
            };

            return Task.CompletedTask;
        }
    }
}