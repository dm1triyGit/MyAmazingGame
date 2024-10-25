using AmazingGameServer.DAL.Abstractions;
using AmazingGameServer.DAL.DataAccess;
using AmazingGameServer.DAL.Repositories;
using AmazingGameServer.DAL.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite();
});

builder.Services.AddScoped<AppDbContextInitialiser>();
builder.Services.AddScoped<IGameRepository, GameRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}
// Configure the HTTP request pipeline.

app.Run();
