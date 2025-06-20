using Microsoft.IdentityModel.Tokens;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Middleware.Security
{
    /// <summary>
    /// Middleware component responsible for validating JWT tokens in the Authorization header of incoming requests.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ITokenService tokenService, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP request.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A <see cref="Task"/> that represents the completion of the middleware processing.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Skip JWT validation for specific authentication-related endpoints.
                if (context.Request.Path.StartsWithSegments("/api/Auth/generate-token") ||
                    context.Request.Path.StartsWithSegments("/api/Auth/register"))
                {
                    await _next(context);
                    return;
                }

                // Try to retrieve the Authorization header from the request.
                if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    // Extract the JWT token from the Bearer scheme.
                    var token = authorizationHeader.ToString().Replace("Bearer ", "");

                    // Check if the token is null or empty.
                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        await context.Response.WriteAsync("Authorization header is missing or invalid.");
                        return;
                    }

                    // Validate the token using TokenService
                    if (_tokenService.ValidateToken(token))
                    {
                        await _next(context);
                        return;
                    }
                    else
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        await context.Response.WriteAsync("Invalid JWT token.");
                        return;
                    }
                }
                else
                {
                    // If the Authorization header is missing for protected endpoints.
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Authorization header is missing.");
                    return;
                }
            }
            catch (Exception ex)
            {
                // Log and handle any unexpected errors during JWT processing.
                _logger.LogError(ex, "An unexpected error occurred during JWT validation.");
                context.Response.StatusCode = 500; // Internal Server Error
                await context.Response.WriteAsync("An unexpected error occurred.");
                return;
            }
        }
    }
}
