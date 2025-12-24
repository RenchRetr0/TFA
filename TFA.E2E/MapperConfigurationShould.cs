using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TFA.E2E;

public class MapperConfigurationShould : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public MapperConfigurationShould(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public void BeValid()
    {
        factory.Services.GetRequiredService<IMapper>()
            .ConfigurationProvider.Invoking(p => p.AssertConfigurationIsValid())
            .Should().NotThrow();
    }
}
