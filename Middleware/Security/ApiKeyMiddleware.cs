using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterChangeApi.Security.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _apiKey;
        
        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;            
            _apiKey = configuration.GetSection("ApiKeys").Get<List<string>>() ?? new List<string>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key Missing");
                return;
            }

            if (_apiKey != extractedApiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await _next(context);
        }
    }
}