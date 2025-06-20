using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeterChangeApi.Middleware.ExceptionHandling
{
    /// <summary>
    /// A global exception filter that handles specific API exceptions and formats the response accordingly.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ApiExceptionFilter"/> class.
    /// </remarks>
    /// <param name="logger">The logger instance for logging exceptions.</param>
    /// <param name="env">The hosting environment to determine if the application is in development mode.</param>
    public class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IWebHostEnvironment env) : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger = logger;
        private readonly IWebHostEnvironment _env = env;

        /// <summary>
        /// Called after an action has thrown an Exception.
        /// </summary>
        /// <param name="context">The <see cref="ExceptionContext"/> which contains information about the exception.</param>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // Handle NotFoundException by returning a 404 Not Found response.
            if (exception is NotFoundException)
            {
                context.Result = new NotFoundResult();
            }
            // Handle DatabaseConflictException by returning a 409 Conflict response with the error message.
            else if (exception is DatabaseConflictException)
            {
                context.Result = new ConflictObjectResult(exception.Message);
            }
            // Handle InvalidInputException by returning a 400 Bad Request response with the error message.
            else if (exception is InvalidInputException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
            }
            // Handle RepositoryException by logging the error and returning a 500 Internal Server Error.
            else if (exception is RepositoryException)
            {
                _logger.LogError(exception, "Repository Exception");
                context.Result = new StatusCodeResult(500);
            }
            // Handle ServiceException by logging the error and returning a 500 Internal Server Error.
            else if (exception is ServiceException)
            {
                _logger.LogError(exception, "Service Exception");
                context.Result = new StatusCodeResult(500);
            }
            // Handle any other unhandled exceptions.
            else
            {
                _logger.LogError(exception, "Unhandled Exception");

                // In development, return a detailed error response including the stack trace.
                if (_env.IsDevelopment())
                {
                    var errorResponse = new
                    {
                        exception.Message,
                        exception.StackTrace
                    };
                    context.Result = new ObjectResult(errorResponse) { StatusCode = 500 };
                }
                // In production, return a generic 500 Internal Server Error.
                else
                {
                    context.Result = new StatusCodeResult(500);
                }
            }

            // Mark the exception as handled to prevent further processing.
            context.ExceptionHandled = true;
        }
    }
}