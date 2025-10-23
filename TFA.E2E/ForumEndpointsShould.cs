using System;
using FluentAssertions;

namespace TFA.E2E;

public class ForumEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;

    public ForumEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ReturnListOfForums()
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.GetAsync("forums");

        response.Invoking(r => r.EnsureSuccessStatusCode()).Should().NotThrow();

        var result = await response.Content.ReadAsStringAsync();
        result.Should().Be("[]");
    }
}
