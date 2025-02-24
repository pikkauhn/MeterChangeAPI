using MeterChangeApi.Data;
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
};


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();