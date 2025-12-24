using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.UseCase.GetForums;
using TFA.Domain.UseCase.GetTopics;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly GetTopicsUseCase sut;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;
    private readonly Mock<IGetTopicsStorage> storage;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Topic> resources, int totalCount)>> getTopicsSetup;

    public GetTopicsUseCaseShould()
    {
        var validator = new Mock<IValidator<GetTopicsQuery>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var getForumsStorage = new Mock<IGetForumsStorage>();
        getForumsSetup = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));

        storage = new Mock<IGetTopicsStorage>();
        getTopicsSetup = storage.Setup(s =>
            s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));

        sut = new GetTopicsUseCase(validator.Object, storage.Object, getForumsStorage.Object);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNotFound()
    {
        var forumId = Guid.Parse("d935d88c-e702-4415-b8a7-4ededaf1e1c0");

        getForumsSetup.ReturnsAsync([new() { Id = Guid.Parse("fd363148-696c-4793-8d28-1c0be4198183"), Title = "Hello world" }]);

        var query = new GetTopicsQuery(forumId, 0, 1);
        await sut.Invoking(s => s.Execute(query, CancellationToken.None))
        .Should().ThrowAsync<ForumNotFoundException>();
    }

    [Fact]
    public async Task ReturnTopics_ExtractedFromStorage_WhenForumExists()
    {
        var forumId = Guid.Parse("4b29e740-891e-417f-a5da-870be911d57c");

        getForumsSetup.ReturnsAsync([new() { Id = Guid.Parse("4b29e740-891e-417f-a5da-870be911d57c"), Title = "Hello world" }]);
        var expectedResources = new Topic[] { new Topic { Title = "test" } };
        var expectedTotalCount = 6;
        getTopicsSetup.ReturnsAsync((expectedResources, expectedTotalCount));

        var (actualResources, actualTotalCount) = await sut.Execute(
            new GetTopicsQuery(forumId, 5, 10), CancellationToken.None);

        actualResources.Should().BeEquivalentTo(expectedResources);
        actualTotalCount.Should().Be(expectedTotalCount);
        storage.Verify(s => s.GetTopics(forumId, 5, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
