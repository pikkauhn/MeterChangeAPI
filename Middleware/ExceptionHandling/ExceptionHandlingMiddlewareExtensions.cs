namespace MeterChangeApi.Middleware.ExceptionHandling
{
    /// <summary>
    /// Extension methods for adding the <see cref="ExceptionHandlingMiddleware"/> to the HTTP request pipeline.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="ExceptionHandlingMiddleware"/> to the application's request pipeline.
        /// This middleware will catch any unhandled exceptions and return a standardized error response.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> so that additional middleware can be chained.</returns>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            // Use the specified middleware type in the application's request pipeline.
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}