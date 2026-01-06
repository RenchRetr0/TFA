using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Models;
using TFA.Domain.UseCase.GetForums;

namespace TFA.Domain.Tests.GetForums;

public class GetForumsUseCaseShould
{
    private readonly Mock<IGetForumsStorage> storage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;
    private readonly GetForumsUseCase sut;

    public GetForumsUseCaseShould()
    {
        storage = new Mock<IGetForumsStorage>();
        getForumsSetup = storage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));

        sut = new GetForumsUseCase(storage.Object);
    }

    [Fact]
    public async Task ReturnForums_FromStorage()
    {
        var forums = new Forum[] {
            new() { Id = Guid.Parse("675e4c98-3470-44df-b8cd-3df3065da00d"), Title = "Test forum 1" },
            new() { Id = Guid.Parse("988fd570-7fdd-443c-971b-58dd1d2ef9fe"), Title = "Test forum 2" }
        };
        getForumsSetup.ReturnsAsync(forums);

        var actual = await sut.Execute(CancellationToken.None);
        actual.Should().BeSameAs(forums);
        storage.Verify(s => s.GetForums(CancellationToken.None), Times.Once);
        storage.VerifyNoOtherCalls();
    }
}
