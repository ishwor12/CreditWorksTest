using Microsoft.EntityFrameworkCore;
using VehicleManagement.Infrastructure.ApplicationDbContext;
using VehicleManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure(connectionString);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
