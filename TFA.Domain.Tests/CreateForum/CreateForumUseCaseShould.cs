using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCase.CreateForum;

namespace TFA.Domain.Tests.CreateForum;

public class CreateForumUseCaseShould
{
    private readonly Mock<ICreateForumStorage> storage;
    private readonly ISetup<ICreateForumStorage, Task<Forum>> createdForumSetup;
    private readonly CreateForumUseCase sut;

    public CreateForumUseCaseShould()
    {
        var validator = new Mock<IValidator<CreateForumCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateForumCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var intentionManager = new Mock<IIntentionManager>();
        intentionManager
            .Setup(m => m.IsAllowed(It.IsAny<ForumIntention>()))
            .Returns(true);

        storage = new Mock<ICreateForumStorage>();
        createdForumSetup = storage.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        sut = new CreateForumUseCase(validator.Object, intentionManager.Object, storage.Object);
    }

    [Fact]
    public async Task ReturnCreateForum()
    {
        var forum = new Forum
        {
            Id = Guid.Parse("124ceddd-29ca-46cd-ae0d-0a819c1d1e9a"),
            Title = "Hello",
        };
        createdForumSetup.ReturnsAsync(forum);

        var actual = await sut.Execute(new CreateForumCommand("Hello"), CancellationToken.None);
        actual.Should().Be(forum);

        storage.Verify(s => s.Create("Hello", It.IsAny<CancellationToken>()), Times.Once);
        storage.VerifyNoOtherCalls();
    }
}
