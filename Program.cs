using MeterChangeApi.Security.Middleware;
using Microsoft.EntityFrameworkCore;

using MeterChangeApi.Data;
using MeterChangeApi.Services;
using MeterChangeApi.Repositories;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Data.Logger;
using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Filters;
using MeterChangeApi.Repositories.Helpers;
using MeterChangeApi.Services.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddProvider(new FileLoggerProvider("logs/MeterChangeApi.txt"));

var connectionString = builder.Configuration["MySqlConnection"];

if (connectionString != null)
{
    builder.Services.AddDbContext<ChangeOutContext>(options =>
    options.UseMySQL(connectionString.ToString()).LogTo(Console.WriteLine, LogLevel.Warning));

}
else
{
    Console.WriteLine("No Connection String Found.");
}
;


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IDatabaseOperationHandler, DatabaseOperationHandler>();
builder.Services.AddScoped<IServiceOperationHandler, ServiceOperationHandler>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IArcGISDataRepository, ArcGISDataRepository>();
builder.Services.AddScoped<IMeterRepository, MeterRepository>();
builder.Services.AddScoped<IEndpointRepository, EndpointRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IArcGISDataService, ArcGISDataService>();
builder.Services.AddScoped<IMeterService, MeterService>();
builder.Services.AddScoped<IEndpointService, EndpointService>();

builder.Services.AddSingleton<ILogger>(provider => provider.GetRequiredService<ILogger<AppLogger>>());
builder.Services.AddScoped<IAppLogger, AppLogger>();
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

logger.LogInformation("Server started at: {time}", DateTime.UtcNow);

// app.UseHttpsRedirection();
app.UseExceptionHandling();
app.UseAuthorization();

app.MapControllers();

// app.UseMiddleware<ApiKeyMiddleware>();

app.Run();