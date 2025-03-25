using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeterChangeApi.Security.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    var token = authorizationHeader.ToString().Replace("Bearer ", "");

                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Authorization header is missing or invalid.");
                        return;
                    }

                    var jwtKey = _configuration["Jwt:Key"];
                    if (string.IsNullOrEmpty(jwtKey))
                    {
                        _logger.LogError("JWT Key is missing or invalid in configuration.");
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("JWT Key is missing or invalid in configuration.");
                        return;
                    }

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(jwtKey);

                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    await _next(context);
                    return;
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Authorization header is missing.");
                    return;
                }
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Invalid JWT token: {Message}", ex.Message);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid JWT token.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during JWT validation.");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
                return;
            }
        }
    }
}