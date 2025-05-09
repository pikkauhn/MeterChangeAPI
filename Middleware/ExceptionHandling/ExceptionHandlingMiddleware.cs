using MeterChangeApi.Data.Logger;
using System.Net;
using System.Text.Json;

namespace MeterChangeApi.Middleware.ExceptionHandling
{
    /// <summary>
    /// Middleware component responsible for handling exceptions that occur during request processing.
    /// It catches exceptions, logs them, and returns a standardized error response to the client.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </remarks>
    /// <param name="next">The next middleware in the pipeline.</param>
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// Invokes the middleware to process the HTTP request.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A <see cref="Task"/> that represents the completion of the middleware processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline. If an exception occurs down the line,
                // it will be caught by the catch block below.
                await _next(context);
            }
            catch (Exception ex)
            {
                // An exception occurred during the processing of the request.
                // Call the HandleExceptionAsync method to log the error and generate the error response.
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception by setting the response status code, content type,
        /// logging the error, and writing a standardized JSON error response to the client.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>A <see cref="Task"/> that represents the completion of the error handling.</returns>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set the content type of the response to application/json.
            context.Response.ContentType = "application/json";
            var response = context.Response;

            // Set the HTTP status code to Internal Server Error (500) as a default for unhandled exceptions.
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create a standardized error response object.
            var errorResponse = new
            {
                Success = false,
                Message = "Internal Server Error."
            };

            // Resolve the application's logger service from the request services.
            var logger = context.RequestServices.GetRequiredService<IAppLogger>();

            // Log the exception with an error level, including the exception details and a generic error message.
            logger.LogError(exception, $"Error occurred: {errorResponse.Message}");

            // Serialize the error response object to JSON.
            var result = JsonSerializer.Serialize(errorResponse);

            // Write the JSON error response to the HTTP response body.
            await context.Response.WriteAsync(result);
        }
    }
}