using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Sharporum.Domain.CustomExceptions;
using Sharporum.Domain.Models;

namespace Sharporum.API.Filters
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

                    if ((int) httpStatusCodeException.StatusCode >= 500)
                    {
                        Log.Error(httpStatusCodeException, httpStatusCodeException.Message);
                    }

                    Log.Warning(httpStatusCodeException.Message);

                    break;
                default:
                    errorDetails.StatusCode = (int) HttpStatusCode.InternalServerError;
                    errorDetails.Message = exception.Message;
                    Log.Error(exception, exception.Message);

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