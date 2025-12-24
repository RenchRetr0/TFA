using System.Net.Http.Json;
using FluentAssertions;
using TFA.API.Models;
// using TFA.Storage;

namespace TFA.E2E;

public class TopicEndpointsShould : IClassFixture<ForumApiApplicationFactory>, IAsyncLifetime
{
    private readonly ForumApiApplicationFactory factory;
    // private readonly Guid forumId = Guid.Parse("fc3f9bb4-6a94-4759-b9a7-c987ac0db272");

    public TopicEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }

    // public async Task InitializeAsync()
    // {
    //     var dbContext = factory.Services.GetRequiredService<ForumDbContext>();
    //     await dbContext.Forums.AddAsync(new Forum { ForumId = forumId, Title = "Test me" });
    //     await dbContext.SaveChangesAsync();
    // }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ReturnForbidden_WhenNotAuthenticated()
    {
        using var httpClient = factory.CreateClient();

        using var forumCreatedResponse = await httpClient.PostAsync("forums",
            JsonContent.Create(new { title = "Test forum" }));
        forumCreatedResponse.EnsureSuccessStatusCode();

        var createdForum = await forumCreatedResponse.Content.ReadFromJsonAsync<Forum>();
        createdForum.Should().NotBeNull();

        var responseMessage = await httpClient.PostAsync($"forums/{createdForum!.Id}/topics",
            JsonContent.Create(new { title = "Hello world" }));
        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}
