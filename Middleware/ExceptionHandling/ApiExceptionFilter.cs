using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Services;

namespace MeterChangeApi.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is NotFoundException)
            {
                context.Result = new NotFoundResult();
            }
            else if (exception is DatabaseConflictException)
            {
                context.Result = new ConflictObjectResult(exception.Message);
            }
            else if (exception is InvalidInputException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
            }
            else if (exception is RepositoryException)
            {
                _logger.LogError(exception, "Repository Exception");
                context.Result = new StatusCodeResult(500);
            }
            else if (exception is ServiceException)
            {
                _logger.LogError(exception, "Service Exception");
                context.Result = new StatusCodeResult(500);
            }
            else
            {
                _logger.LogError(exception, "Unhandled Exception");

                if (_env.IsDevelopment())
                {
                    var errorResponse = new
                    {
                        Message = exception.Message,
                        StackTrace = exception.StackTrace
                    };
                    context.Result = new ObjectResult(errorResponse) { StatusCode = 500 };
                }
                else
                {
                    context.Result = new StatusCodeResult(500);
                }
            }

            context.ExceptionHandled = true;
        }
    }
}