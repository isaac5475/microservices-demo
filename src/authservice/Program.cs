using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserManagementAPI.Data;
using UserManagementAPI.Extensions;
using UserManagementAPI.Repositories;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1"
    });
});


var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<RefreshTokenService>();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var jwtExpiresHours = Environment.GetEnvironmentVariable("JWT_EXPIRES_HOURS") != null
        ? int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRES_HOURS")!)
        : 12;
var jwtRefreshTokenExpiresDays = Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRES_DAYS") != null
        ? int.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRES_DAYS")!)
        : 14;
builder.Services.Configure<JwtOptions>(options =>
{
    options.SecretKey = jwtSecret!;
    options.ExpiresHours = jwtExpiresHours;
    options.RefreshTokenExpiresDays = jwtRefreshTokenExpiresDays;

});

var jwtOptions = new JwtOptions
{
    SecretKey = jwtSecret!,
    ExpiresHours = jwtExpiresHours,
    RefreshTokenExpiresDays = jwtRefreshTokenExpiresDays
};


builder.Services.AddApiAuthentication(Options.Create(jwtOptions));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        c.RoutePrefix = string.Empty; 
    });
    
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
