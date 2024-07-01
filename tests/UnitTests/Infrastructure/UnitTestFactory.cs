using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SmartFridgeManagerAPI.Application.Auth.Services;
using SmartFridgeManagerAPI.Application.Common.Behaviours;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.Infrastructure.Common.RabbitMq;
using SmartFridgeManagerAPI.Infrastructure.Common.Redis;
using SmartFridgeManagerAPI.Infrastructure.Data;

namespace SmartFridgeManagerAPI.UnitTests.Infrastructure;

public abstract class UnitTestFactory<TRequest, THandler>
{
    protected readonly IAuthEmailService _authEmailService;
    protected readonly AppDbContext _dbContext;
    protected readonly IMapper _mapper;
    protected readonly IMediator _mediator;
    protected readonly IPasswordHasher<User> _passwordHasher;
    protected readonly IRabbitMqService _rabbitMq;
    protected readonly IRedisService _redis;

    protected UnitTestFactory()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        ServiceProvider services = new ServiceCollection()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            })
            .AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddAutoMapper(typeof(TRequest))
            .AddValidatorsFromAssemblyContaining<TRequest>()
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddScoped<IRabbitMqService>(_ => Substitute.For<IRabbitMqService>())
            .AddScoped<IRedisService>(_ => Substitute.For<IRedisService>())
            .AddScoped<IAuthEmailService>(_ => Substitute.For<IAuthEmailService>())
            .AddLocalization()
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<THandler>();
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateBehaviour<,>));
            })
            .BuildServiceProvider();

        _dbContext = services.GetService<AppDbContext>()!;
        _mediator = services.GetService<IMediator>()!;
        _mapper = services.GetService<IMapper>()!;
        _passwordHasher = services.GetService<IPasswordHasher<User>>()!;
        _rabbitMq = services.GetService<IRabbitMqService>()!;
        _redis = services.GetService<IRedisService>()!;
        _authEmailService = services.GetService<IAuthEmailService>()!;
    }
}
