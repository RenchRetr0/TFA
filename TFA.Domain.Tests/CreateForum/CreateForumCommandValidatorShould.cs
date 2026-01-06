using FluentAssertions;
using TFA.Domain.UseCase.CreateForum;

namespace TFA.Domain.Tests.CreateForum;

public class CreateForumCommandValidatorShould
{
    private readonly CreateForumCommandValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var validCommand = new CreateForumCommand("Title");
        sut.Validate(validCommand).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateForumCommand("Title");
        yield return new object[] { validCommand with { Title = string.Empty } };
        yield return new object[] { validCommand with { Title = "123456789012345678901234567890123456789012345678901234567890" } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailed_WhenCommandInvalid(CreateForumCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }
}
