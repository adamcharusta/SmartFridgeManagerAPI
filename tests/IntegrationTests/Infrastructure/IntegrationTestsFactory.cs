using MediatR;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.EntityFrameworkCore;
using SmartFridgeManagerAPI.Infrastructure.Common.RabbitMq;
using SmartFridgeManagerAPI.Infrastructure.Common.Redis;
using SmartFridgeManagerAPI.Infrastructure.Data;

namespace SmartFridgeManagerAPI.IntegrationTests.Infrastructure;

public abstract class IntegrationTestsFactory : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly IMediator _mediatorFake;


    protected IntegrationTestsFactory(WebApplicationFactory<Program> factory)
    {
        _mediatorFake = Substitute.For<IMediator>();
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                ServiceDescriptor? dbContextOptions = services
                    .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (dbContextOptions != null)
                {
                    services.Remove(dbContextOptions);
                }

                ServiceDescriptor? mediator = services
                    .SingleOrDefault(service => service.ServiceType == typeof(IMediator));

                if (mediator != null)
                {
                    services.Remove(mediator);
                }

                ServiceDescriptor? rabbitMq = services
                    .SingleOrDefault(service => service.ServiceType == typeof(IRabbitMqService));

                if (rabbitMq != null)
                {
                    services.Remove(rabbitMq);
                }

                ServiceDescriptor? redis = services
                    .SingleOrDefault(service => service.ServiceType == typeof(IRedisService));

                if (redis != null)
                {
                    services.Remove(redis);
                }

                services.AddScoped<IMediator>(_ => _mediatorFake);
                services.AddScoped<IRabbitMqService>(_ => Substitute.For<IRabbitMqService>());
                services.AddScoped<IRedisService>(_ => Substitute.For<IRedisService>());

                services
                    .AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));
            });
        }).CreateClient();
    }
}
