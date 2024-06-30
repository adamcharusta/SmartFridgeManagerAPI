using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartFridgeManagerAPI.Application.Common.Behaviours;
using SmartFridgeManagerAPI.Domain.Entities;
using SmartFridgeManagerAPI.Infrastructure;
using SmartFridgeManagerAPI.Infrastructure.Services;

namespace SmartFridgeManagerAPI.UnitTests.Infrastructure;

public abstract class UnitTestFactory<THandler>
{
    protected readonly AppDbContext _dbContext;
    protected readonly IMapper _mapper;
    protected readonly IMediator _mediator;
    protected readonly IPasswordHasher<User> _passwordHasher;
    protected readonly IRabbitMqService _rabbitMq;

    protected UnitTestFactory()
    {
        ServiceProvider services = new ServiceCollection()
            .AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddScoped<IRabbitMqService>(_ => Substitute.For<IRabbitMqService>())
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
    }
}
