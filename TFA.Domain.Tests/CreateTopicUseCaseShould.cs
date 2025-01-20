using FluentAssertions;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCase.CreateTopic;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;

namespace TFA.Domain.Tests;

public class CreateTopicUseCaseShould
{
    private readonly CreateTopicUseCase sut;
    private readonly Mock<ICreateTopicStorage> storage;
    private readonly ISetup<ICreateTopicStorage, Task<bool>> forumExistSetup;
    private readonly ISetup<ICreateTopicStorage, Task<Models.Topic>> createTopicSetup;
    private readonly ISetup<IIdentity, Guid> getCurrentUserIdSetup;
    private readonly Mock<IIntentionManager> intentionManager;
    private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;

    public CreateTopicUseCaseShould()
    {
        storage = new Mock<ICreateTopicStorage>();
        forumExistSetup = storage.Setup(s => s.ForumExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
        createTopicSetup = storage.Setup(s =>
            s.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        
        var identity = new Mock<IIdentity>();
        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider.Setup(p => p.Current).Returns(identity.Object);
        getCurrentUserIdSetup = identity.Setup(s => s.UserId);

        intentionManager = new Mock<IIntentionManager>();
        intentionIsAllowedSetup = intentionManager.Setup(m => m.IsAllowed(It.IsAny<TopicIntention>()));

        sut = new CreateTopicUseCase(intentionManager.Object, identityProvider.Object, storage.Object);
    }

    [Fact]
    public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
    {
        var forumId = Guid.Parse("11d66ac0-bf3c-4e7b-a97b-2f371edf4f77");
        
        intentionIsAllowedSetup.Returns(false);

        await sut.Invoking(s => s.Execute(forumId, "Whatever", CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerExtension>();
        intentionManager.Verify(m => m.IsAllowed(TopicIntention.Create));
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("da940b4e-95a1-4f6e-a1a7-24096997b24d");
        
        intentionIsAllowedSetup.Returns(true);
        forumExistSetup.ReturnsAsync(false);
        
        await sut.Invoking(s => s.Execute(forumId, "Some Title", CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
        storage.Verify(s => s.ForumExists(forumId, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExits()
    {
        var forumId = Guid.Parse("2a0721f3-e2e9-45ab-973f-d9541db5753e");
        var userId = Guid.Parse("db9615bb-3b4b-47e2-a27e-a2fd6b682654");
        
        intentionIsAllowedSetup.Returns(true);
        forumExistSetup.ReturnsAsync(true);
        getCurrentUserIdSetup.Returns(userId);
        var expected = new Models.Topic();
        createTopicSetup.ReturnsAsync(expected);

        var actual = await sut.Execute(forumId, "Hello world", CancellationToken.None);
        actual.Should().Be(expected);

        storage.Verify(s => s.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
    }
}
