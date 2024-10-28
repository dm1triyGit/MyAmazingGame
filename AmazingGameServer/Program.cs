using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Options;
using AmazingGameServer.BLL.Services;
using AmazingGameServer.DAL.Abstractions;
using AmazingGameServer.DAL.DataAccess;
using AmazingGameServer.DAL.Repositories;
using AmazingGameServer.DAL.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite();
});
builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("game");
});

var authOptions = builder.Configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = authOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Key)),
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddGrpc();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddScoped<AppDbContextInitialiser>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

app.MapControllers();
app.MapGrpcService<GameSessionService>();

app.Run();
