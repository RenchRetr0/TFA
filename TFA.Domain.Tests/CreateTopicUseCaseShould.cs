using FluentAssertions;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCase.CreateTopic;
using Microsoft.EntityFrameworkCore;
using Moq;
using TFA.Storage;
using Moq.Language.Flow;

namespace TFA.Domain.Tests;

public class CreateTopicUseCaseShould
{
    private readonly ForumDbContext forumDbContext;
    private readonly ISetup<IGuidFactory, Guid> createIdSetup;
    private readonly ISetup<IMomentProvider, DateTimeOffset> getNowSetup;
    private readonly CreateTopicUseCase sut;
    
    public CreateTopicUseCaseShould()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(nameof(CreateTopicUseCaseShould));
        forumDbContext = new ForumDbContext(dbContextOptionsBuilder.Options);

        var guidFactory = new Mock<IGuidFactory>();
        createIdSetup = guidFactory.Setup(f => f.Create());

        var momentProvider = new Mock<IMomentProvider>();
        getNowSetup = momentProvider.Setup(p => p.Now);

        sut = new CreateTopicUseCase(guidFactory.Object, momentProvider.Object, forumDbContext);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        await forumDbContext.Forums.AddAsync(new Storage.Forum
        {
            ForumId = Guid.Parse("22d0e623-87cb-4685-8433-bcc3506711d9"),
            Title = "Basic forum"
        });

        await forumDbContext.SaveChangesAsync();
        
        var forumId = Guid.Parse("da940b4e-95a1-4f6e-a1a7-24096997b24d");
        var authorId = Guid.Parse("22f013b4-8df0-4b0e-8505-fcd6f75af6f0");
        
        await sut.Invoking(s => s.Execute(forumId, "Some Title", authorId, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic()
    {
        var forumId = Guid.Parse("2a0721f3-e2e9-45ab-973f-d9541db5753e");
        var userId = Guid.Parse("db9615bb-3b4b-47e2-a27e-a2fd6b682654");

        await forumDbContext.Forums.AddAsync(new Storage.Forum
        {
            ForumId = forumId,
            Title = "Existing forum"
        });

        await forumDbContext.Users.AddAsync(new User{
            UserId = userId,
            Login = "Aiden"
        });
        await forumDbContext.SaveChangesAsync();

        createIdSetup.Returns(Guid.Parse("33a7c12c-842a-43f8-b39b-bec3f1dd9fbe"));
        getNowSetup.Returns(new DateTimeOffset(2025, 01, 18, 05, 21, 0, TimeSpan.FromHours(2)));
        
        var actual = await sut.Execute(forumId, "Hello world", userId, CancellationToken.None);
        var allTopics = await forumDbContext.Topics.ToArrayAsync();
        allTopics.Should().BeEquivalentTo(new []
        {
            new Storage.Topic
            {
                ForumId = forumId,
                UserId = userId,
                Title = "Hello world"
            }
        }, cfg => cfg.Including(t => t.ForumId).Including(t => t.UserId).Including(t => t.Title));
        actual.Should().BeEquivalentTo(new Models.Topic
        {
            Id = Guid.Parse("33a7c12c-842a-43f8-b39b-bec3f1dd9fbe"),
            Title = "Hello world",
            Author = "Aiden",
            CreatedAt = new DateTimeOffset(2025, 01, 18, 05, 21, 0, TimeSpan.FromHours(2))
        });
    }
}
