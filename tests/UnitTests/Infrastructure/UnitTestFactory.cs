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

namespace SmartFridgeManagerAPI.UnitTests.Infrastructure;

public abstract class UnitTestFactory<THandler>
{
    protected readonly AppDbContext _dbContext;
    protected readonly IMapper _mapper;
    protected readonly IMediator _mediator;
    protected readonly IPasswordHasher<User> _passwordHasher;
    protected readonly ServiceProvider _services;

    protected UnitTestFactory()
    {
        _services = new ServiceCollection()
            .AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<THandler>();
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateBehaviour<,>));
            })
            .BuildServiceProvider();

        _dbContext = _services.GetService<AppDbContext>()!;
        _mediator = _services.GetService<IMediator>()!;
        _mapper = _services.GetService<IMapper>()!;
        _passwordHasher = _services.GetService<IPasswordHasher<User>>()!;
    }
}
