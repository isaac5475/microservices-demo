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

// Ensure database is created on startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    
    int maxRetries = 30;
    int retryDelayMs = 1000;
    bool connected = false;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation($"Attempting to connect to database (attempt {attempt}/{maxRetries})...");
            
            dbContext.Database.OpenConnection();
            dbContext.Database.CloseConnection();
            
            connected = true;
            logger.LogInformation("Successfully connected to database");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Database connection attempt {attempt} failed: {ex.Message}");
            
            if (attempt < maxRetries)
            {
                logger.LogInformation($"Waiting {retryDelayMs}ms before retry...");
                Thread.Sleep(retryDelayMs);
            }
            else
            {
                logger.LogError("Max retries reached. Could not connect to database.");
                throw;
            }
        }
    }
    
    if (!connected)
    {
        throw new Exception("Failed to connect to database after multiple retries");
    }
    
    logger.LogInformation("Creating database tables if they don't exist...");
    
    try
    {
        // Ensure public schema exists
        dbContext.Database.ExecuteSqlRaw(@"
            CREATE SCHEMA IF NOT EXISTS public;
        ");
        
        // Create tables if they don't exist
        dbContext.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS public.""Users"" (
                ""Id"" UUID PRIMARY KEY,
                ""Username"" TEXT UNIQUE NOT NULL,
                ""Email"" TEXT UNIQUE NOT NULL,
                ""PasswordHash"" TEXT NOT NULL
            );
        ");
        
        logger.LogInformation("Users table created/verified");
        
        dbContext.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS public.""RefreshTokens"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Token"" UUID UNIQUE NOT NULL,
                ""Expires"" TIMESTAMP NOT NULL,
                ""UserId"" UUID NOT NULL,
                FOREIGN KEY (""UserId"") REFERENCES public.""Users""(""Id"") ON DELETE CASCADE
            );
        ");
        
        logger.LogInformation("RefreshTokens table created/verified");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating database tables");
        throw;
    }
}

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
