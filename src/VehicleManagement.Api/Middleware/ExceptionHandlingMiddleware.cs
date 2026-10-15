using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Application.Exceptions;

namespace VehicleManagement.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationFailedException ex)
            {
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Errors);
            }
            catch (NotFoundException ex)
            {
                await WriteResponseAsync(context, StatusCodes.Status404NotFound, "Not found", new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                // The full error goes to the log but ui only display's  generic message.
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, int statusCode, string title, List<string> errors)
        {
            var problem = new ProblemDetails { Status = statusCode, Title = title };
            problem.Extensions["errors"] = errors;

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
