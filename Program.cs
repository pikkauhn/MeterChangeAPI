using MeterChangeApi.Security.Middleware;
using Microsoft.EntityFrameworkCore;

using MeterChangeApi.Data;
using MeterChangeApi.Services;
using MeterChangeApi.Repositories;
using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeAPI.Repositories.Interfaces;
using MeterChangeAPI.Repositories;
using MeterChangeAPI.Services.Interfaces;
using MeterChangeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["MySqlConnection"];

// Add services to the container.
if (connectionString != null)
{
    builder.Services.AddDbContext<ChangeOutContext>(options =>
    options.UseMySQL(connectionString.ToString()));
}
else
{
    Console.WriteLine("No Connection String Found.");
}
;


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IArcGISDataRepository, ArcGISDataRepository>();
builder.Services.AddScoped<IMeterRepository, MeterRepository>();
builder.Services.AddScoped<IEndpointRepository, EndpointRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IArcGISDataService, ArcGISDataService>();
builder.Services.AddScoped<IMeterService, MeterService>();
builder.Services.AddScoped<IEndpointService, EndpointService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// app.UseMiddleware<ApiKeyMiddleware>();

app.Run();