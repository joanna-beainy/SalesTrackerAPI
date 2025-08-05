using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Exceptions;
using SalesTracker.Shared.Responses;
using Serilog;
using System.Text.Json;

namespace SalesTrackerAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var response = ApiResponse<string>.Fail(ex.Message);
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception caught by GlobalExceptionMiddleware");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = ApiResponse<string>.Fail(APIMessages.ServerError);
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }

        }
    }

}
