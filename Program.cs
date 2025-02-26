using MeterChangeApi.Data;
using MeterChangeApi.Repositories.Interfaces;
using MeterChangeApi.Services.Interfaces;
using MeterChangeAPI.Repository;
using MeterChangeAPI.Security.Middleware;
using MeterChangeAPI.Services;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<IAddressService, AddressService>();

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

app.UseMiddleware<ApiKeyMiddleware>();

app.Run();