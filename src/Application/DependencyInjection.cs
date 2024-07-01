using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartFridgeManagerAPI.Application.Auth.Services;
using SmartFridgeManagerAPI.Application.Auth.Settings;
using SmartFridgeManagerAPI.Application.Common.Behaviours;
using SmartFridgeManagerAPI.Domain.Entities;

namespace SmartFridgeManagerAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddServices()
            .AddAuthServices(configuration)
            .AddLocalization();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthEmailService, AuthEmailService>()
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateBehaviour<,>));
            });
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

        services
            .AddSingleton(authSettings)
            .AddAuthentication(opt =>
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

    private static IServiceCollection AddLocalization(this IServiceCollection services)
    {
        services.AddLocalization(opt => opt.ResourcesPath = "Resources");
        services.Configure<RequestLocalizationOptions>(opt =>
        {
            string[] supportedCultures = ["en-US", "pl-PL"];
            opt.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });

        return services;
    }

    public static WebApplication UseApplication(this WebApplication app)
    {
        app.UseRequestLocalization();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
