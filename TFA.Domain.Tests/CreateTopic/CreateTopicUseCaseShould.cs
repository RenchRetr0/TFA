using FluentAssertions;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCase.CreateTopic;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using FluentValidation;
using FluentValidation.Results;
using TFA.Domain.UseCase.GetForums;
using TFA.Domain.Models;

namespace TFA.Domain.Tests.CreateTopic;

public class CreateTopicUseCaseShould
{
    private readonly CreateTopicUseCase sut;
    private readonly Mock<ICreateTopicStorage> storage;
    private readonly ISetup<ICreateTopicStorage, Task<Topic>> createTopicSetup;
    private readonly Mock<IGetForumsStorage> getForumsStorage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;
    private readonly ISetup<IIdentity, Guid> getCurrentUserIdSetup;
    private readonly Mock<IIntentionManager> intentionManager;
    private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;

    public CreateTopicUseCaseShould()
    {
        storage = new Mock<ICreateTopicStorage>();
        createTopicSetup = storage.Setup(s =>
            s.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

        getForumsStorage = new Mock<IGetForumsStorage>();
        getForumsSetup = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));

        var identity = new Mock<IIdentity>();
        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider.Setup(p => p.Current).Returns(identity.Object);
        getCurrentUserIdSetup = identity.Setup(s => s.UserId);

        intentionManager = new Mock<IIntentionManager>();
        intentionIsAllowedSetup = intentionManager.Setup(m => m.IsAllowed(It.IsAny<TopicIntention>()));

        var validator = new Mock<IValidator<CreateTopicCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        sut = new(validator.Object, intentionManager.Object, identityProvider.Object, getForumsStorage.Object, storage.Object);
    }

    [Fact]
    public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
    {
        var forumId = Guid.Parse("11d66ac0-bf3c-4e7b-a97b-2f371edf4f77");

        intentionIsAllowedSetup.Returns(false);

        await sut.Invoking(s => s.Execute(new CreateTopicCommand(forumId, "Whatever"), CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerException>();
        intentionManager.Verify(m => m.IsAllowed(TopicIntention.Create));
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("da940b4e-95a1-4f6e-a1a7-24096997b24d");

        intentionIsAllowedSetup.Returns(true);
        getForumsSetup.ReturnsAsync(Array.Empty<Forum>());

        await sut.Invoking(s => s.Execute(new CreateTopicCommand(forumId, "Some Title"), CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExits()
    {
        var forumId = Guid.Parse("2a0721f3-e2e9-45ab-973f-d9541db5753e");
        var userId = Guid.Parse("db9615bb-3b4b-47e2-a27e-a2fd6b682654");
        var titlePublic = "Hello world";

        intentionIsAllowedSetup.Returns(true);
        getForumsSetup.ReturnsAsync(new Forum[] { new Forum { Id = forumId, Title = titlePublic } });
        getCurrentUserIdSetup.Returns(userId);
        var expected = new Models.Topic();
        createTopicSetup.ReturnsAsync(expected);

        var actual = await sut.Execute(new CreateTopicCommand(forumId, titlePublic), CancellationToken.None);
        actual.Should().Be(expected);

        storage.Verify(s => s.CreateTopic(forumId, userId, titlePublic, It.IsAny<CancellationToken>()), Times.Once);
    }
}
