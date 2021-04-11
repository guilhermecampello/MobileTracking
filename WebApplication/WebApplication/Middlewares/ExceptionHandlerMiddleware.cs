using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApplication.Application;

namespace WebApplication.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (NotFoundException exception)
            {
                await this.HandleNotFoundException(httpContext, exception);
            }
            catch (InvalidParametersException exception)
            {
                await this.HandleInvalidParametersException(httpContext, exception);
            }
            catch (Exception exception)
            {
                await this.HandleUnexpectedException(httpContext, exception);
            }
        }

        private Task HandleNotFoundException(
            HttpContext httpContext, NotFoundException exception)
        {
            return this.HandleProblem(
                httpContext,
                $"{exception.Type.Name} not found",
                StatusCodes.Status404NotFound,
                exception);
        }

        private Task HandleInvalidParametersException(
            HttpContext httpContext, InvalidParametersException exception)
        {
            return this.HandleProblem(
                httpContext,
                "Invalid parameters",
                StatusCodes.Status422UnprocessableEntity,
                exception);
        }

        private Task HandleUnexpectedException(
        HttpContext httpContext, Exception exception)
        {
            return this.HandleProblem(
                httpContext,
                exception.Message,
                StatusCodes.Status500InternalServerError,
                exception);
        }

        private Task HandleProblem(
            HttpContext httpContext, string title, int statusCode, Exception exception)
        {
            var problemDetails = new ProblemDetails()
            {
                Title = title,
                Detail = exception.Message,
                Status = statusCode
            };

            var responseBody = JsonConvert.SerializeObject(
                problemDetails,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            return httpContext.Response.WriteAsync(responseBody);
        }
    }
}
