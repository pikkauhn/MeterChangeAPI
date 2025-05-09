using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeterChangeApi.Middleware.Security
{
    /// <summary>
    /// Middleware component responsible for validating JWT tokens in the Authorization header of incoming requests.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="configuration">The application's configuration for retrieving JWT settings.</param>
    /// <param name="logger">The logger instance for logging messages within this middleware.</param>
    public class JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<JwtMiddleware> _logger = logger;

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
                    await _next(context); // Call the next middleware in the pipeline.
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
                        context.Response.StatusCode = 401; // Unauthorized.
                        await context.Response.WriteAsync("Authorization header is missing or invalid.");
                        return;
                    }

                    // Retrieve the JWT secret key from the configuration.
                    var jwtKey = _configuration["Jwt:Key"];
                    if (string.IsNullOrEmpty(jwtKey))
                    {
                        _logger.LogError("JWT Key is missing or invalid in configuration.");
                        context.Response.StatusCode = 500; // Internal Server Error.
                        await context.Response.WriteAsync("JWT Key is missing or invalid in configuration.");
                        return;
                    }

                    // Initialize the JWT token handler and the signing key.
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(jwtKey);

                    try
                    {
                        // Validate the JWT token against the configured parameters.
                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidIssuer = _configuration["Jwt:Issuer"],
                            ValidAudience = _configuration["Jwt:Audience"],
                            ClockSkew = TimeSpan.Zero // Recommended to avoid issues with server/client time differences.
                        }, out SecurityToken validatedToken);

                        // If validation succeeds, call the next middleware in the pipeline.
                        await _next(context);
                        return;
                    }
                    catch (SecurityTokenException ex)
                    {
                        // Log and handle invalid JWT token exceptions.
                        _logger.LogError(ex, "Invalid JWT token: {Message}", ex.Message);
                        context.Response.StatusCode = 401; // Unauthorized.
                        await context.Response.WriteAsync("Invalid JWT token.");
                        return;
                    }
                }
                else
                {
                    // If the Authorization header is missing for protected endpoints.
                    context.Response.StatusCode = 401; // Unauthorized.
                    await context.Response.WriteAsync("Authorization header is missing.");
                    return;
                }
            }
            catch (Exception ex)
            {
                // Log and handle any unexpected errors during JWT processing.
                _logger.LogError(ex, "An unexpected error occurred during JWT validation.");
                context.Response.StatusCode = 500; // Internal Server Error.
                await context.Response.WriteAsync("An unexpected error occurred.");
                return;
            }
        }
    }
}