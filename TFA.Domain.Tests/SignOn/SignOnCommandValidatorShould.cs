using FluentAssertions;
using TFA.Domain.UseCase.SignOn;

namespace TFA.Domain.Tests.SignOn;

public class SignOnCommandValidatorShould
{
    private readonly SignOnCommandValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var validCommand = new SignOnCommand("Test", "test228");
        sut.Validate(validCommand).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new SignOnCommand("Test", "test228");
        yield return new object[] { validCommand with { Login = string.Empty } };
        yield return new object[] { validCommand with { Login = "123456789012345678901" } };
        yield return new object[] { validCommand with { Login = "   " } };
        yield return new object[] { validCommand with { Password = string.Empty } };
        yield return new object[] { validCommand with { Password = "    " } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandInvalid(SignOnCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }
}
