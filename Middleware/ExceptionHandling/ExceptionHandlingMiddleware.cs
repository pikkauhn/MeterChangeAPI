using MeterChangeApi.Data.Logger;
using System.Net;
using System.Text.Json;

namespace MeterChangeApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                Success = false,
                Message = "Internal Server Error."
            };
            
            var logger = context.RequestServices.GetRequiredService<IAppLogger>();
            logger.LogError(exception, $"Error occurred: {errorResponse.Message}");

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}