using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // 2. UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. JWT Settings
        services.Configure<JwtSettings>(
            configuration.GetSection("JwtSettings"));

        // 4. JwtService
        services.AddScoped<IJwtService, JwtService>();

        // 5. JWT Authentication — with null safety!
        var jwtSettings = configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>() ?? new JwtSettings();

        // Only configure JWT if secret exists
        if (!string.IsNullOrEmpty(jwtSettings.Secret))
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            });
        }
        else
        {
            // Still register authentication without validation
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
        }

        // 6. Authorization
        services.AddAuthorization();

        // 7. Redis — with retry policy!
        var redisConnection = configuration
            .GetConnectionString("Redis") ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = ConfigurationOptions.Parse(redisConnection);
            config.AbortOnConnectFail = false; // ← don't crash if Redis not ready!
            config.ConnectRetry = 3;
            config.ReconnectRetryPolicy = new ExponentialRetry(5000);
            return ConnectionMultiplexer.Connect(config);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "ECommerce:";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
