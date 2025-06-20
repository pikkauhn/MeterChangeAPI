using MeterChangeApi.Data;
using MeterChangeApi.Data.Logger;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Middleware.Security;
using MeterChangeApi.Options;
using MeterChangeApi.Options.Config;
using MeterChangeApi.Repositories;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services;
using MeterChangeApi.Services.CsvImport;
using MeterChangeApi.Services.Helpers;
using MeterChangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure file logging provider. Logs will be written to "logs/MeterChangeApi.txt".
builder.Logging.AddProvider(new FileLoggerProvider("logs/MeterChangeApi.txt"));

// Retrieve the database connection string from configuration.
var connectionString = builder.Configuration["MySqlConnection"];

// Configure Entity Framework Core to use MySQL if a connection string is found.
if (connectionString != null)
{
    builder.Services.AddDbContext<ChangeOutContext>(options =>
        options.UseMySQL(connectionString.ToString()).LogTo(Console.WriteLine, LogLevel.Warning)); // Log EF Core warnings to the console.
}
else
{
    Console.WriteLine("No Connection String Found.");
}
;

// Configure the DevelopmentOptions from the "Development" section of the configuration.
builder.Services.Configure<DevelopmentOptions>(builder.Configuration.GetSection("Development"));
var devOptions = builder.Configuration.GetSection("Development").Get<DevelopmentOptions>();

// Conditionally enable Swagger based on the DevelopmentOptions.
if (devOptions?.EnableSwagger == true)
{
    builder.Services.ConfigureSwagger(devOptions.Swagger);
}

// Add controllers and apply the ApiExceptionFilter globally to handle exceptions.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Existing Swagger config...

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

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
            Array.Empty<string>()
        }
    });
});

// Add a hosted service to generate a JWT key if one doesn't exist.
builder.Services.AddHostedService<JwtKeyGeneratorService>();

// Register scoped services for database operation handling.
builder.Services.AddScoped<MeterChangeApi.Repositories.Interfaces.IDatabaseOperationHandler, MeterChangeApi.Repositories.Helpers.DatabaseOperationHandler>();
builder.Services.AddScoped<IServiceOperationHandler, ServiceOperationHandler>();

// Register scoped services for  CSV Importing
builder.Services.AddScoped<ICsvImportService, CsvImportService>();

// Configure the JwtOptions from the "Jwt" section of the configuration.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<ITokenService, TokenService>();

// Register scoped services for token and user management.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Register scoped service for Syncronization.
builder.Services.AddScoped<ISyncService, SyncService>();

// Register scoped repositories for data access.
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IArcGISDataRepository, ArcGISDataRepository>();
builder.Services.AddScoped<IMeterRepository, MeterRepository>();
builder.Services.AddScoped<IEndpointRepository, EndpointRepository>();

// Register scoped services for business logic.
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IArcGISDataService, ArcGISDataService>();
builder.Services.AddScoped<IMeterService, MeterService>();
builder.Services.AddScoped<IEndpointService, EndpointService>();

// Register logging services.
builder.Services.AddSingleton<ILogger>(provider => provider.GetRequiredService<ILogger<AppLogger>>());
builder.Services.AddScoped<IAppLogger, AppLogger>();

// Configure JWT authentication.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Retrieve the JWT security key from the configuration.
        var jwt = GetJwtSecurityKey(builder.Configuration);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = jwt, // Set the retrieved security key.
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

// Helper function to retrieve and create the JWT security key.
static SymmetricSecurityKey GetJwtSecurityKey(IConfiguration configuration)
{
    var jwtKey = configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT Key is missing or invalid in configuration.");
    }
    return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
}

var app = builder.Build();

// Get a logger instance for the Program class.
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Conditionally enable Swagger UI in development if configured.
if (app.Environment.IsDevelopment() && app.Services.GetRequiredService<IOptions<DevelopmentOptions>>().Value.EnableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

// Log the server start time.
logger.LogInformation("Server started at: {time}", DateTime.UtcNow);

// app.UseHttpsRedirection(); // Uncomment to enforce HTTPS.
app.UseExceptionHandling(); // Use custom exception handling middleware.
app.UseAuthentication(); // Enable authentication.
app.UseAuthorization(); // Enable authorization.

// Use custom JWT middleware for token validation.
app.UseMiddleware<JwtMiddleware>();

app.MapControllers(); // Map controller routes.

app.Run(); // Start the application.