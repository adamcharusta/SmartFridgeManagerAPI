using MediatR;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SmartFridgeManagerAPI.Infrastructure;

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

                services.AddScoped<IMediator>(_ => _mediatorFake);

                services
                    .AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));
            });
        }).CreateClient();
    }
}
