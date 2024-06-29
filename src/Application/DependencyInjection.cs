using System.Reflection;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartFridgeManagerAPI.Application.Common.Behaviours;
using SmartFridgeManagerAPI.Application.Common.Settings;
using SmartFridgeManagerAPI.Domain.Entities;

namespace SmartFridgeManagerAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateBehaviour<,>));
        });

        services.AddAuthServices(configuration);

        return services;
    }

    private static IServiceCollection AddAuthServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        string jwtKey = Guard.Against.NullOrEmpty(configuration["JWT_KEY"], "JWT_KEY");
        string jwtExpireDays = Guard.Against.NullOrEmpty(configuration["JWT_EXPIRE_DAYS"], "JWT_EXPIRE_DAYS");
        string jwtIssuer = Guard.Against.NullOrEmpty(configuration["JWT_ISSUER"], "JWT_ISSUER");

        AuthSettings authSettings = new()
        {
            JwtIssuer = jwtIssuer, JwtKey = jwtKey, JwtExpireDays = Convert.ToInt32(jwtExpireDays)
        };

        services.AddSingleton(authSettings);

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = "Bearer";
            opt.DefaultScheme = "Bearer";
            opt.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authSettings.JwtIssuer,
                ValidAudience = authSettings.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
            };
        });

        services.AddAuthorization();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }

    public static WebApplication UseApplication(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
