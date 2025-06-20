using Microsoft.OpenApi.Models;

namespace MeterChangeApi.Options.Config
{
    /// <summary>
    /// Provides extension methods for configuring Swagger/OpenAPI services.
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configures the Swagger/OpenAPI generation for the application.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Swagger services to.</param>
        /// <param name="swaggerOptions">The <see cref="SwaggerOptions"/> containing Swagger-specific settings.</param>
        public static void ConfigureSwagger(this IServiceCollection services, SwaggerOptions swaggerOptions)
        {
            // Add Swagger Gen service to the service collection.
            services.AddSwaggerGen(c =>
            {
                // Configure the Swagger document info (title and version).
                c.SwaggerDoc(swaggerOptions.Version, new OpenApiInfo
                {
                    Title = swaggerOptions.Title,
                    Version = swaggerOptions.Version
                });

                // Check if authorization should be enabled in Swagger UI.
                if (swaggerOptions.EnableAuthorization)
                {
                    // Define the security scheme for Bearer token authentication.
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header, // Token will be passed in the Authorization header.
                        Type = SecuritySchemeType.Http, // Use the HTTP Bearer scheme.
                        Scheme = "bearer"
                    });

                    // Apply the security requirement globally, making the "Authorize" button appear in Swagger UI.
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>() // An empty array here applies it globally.
                        }
                    });
                }
            });
        }
    }
}