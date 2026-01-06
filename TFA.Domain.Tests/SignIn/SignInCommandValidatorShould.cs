using FluentAssertions;
using TFA.Domain.UseCase.SignIn;

namespace TFA.Domain.Tests.SignIn;

public class SignInCommandValidatorShould
{
    private readonly SignInCommandValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var command = new SignInCommand("Test", "test228");
        sut.Validate(command).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var command = new SignInCommand("Test", "test228");
        yield return new object[] { command with { Login = string.Empty } };
        yield return new object[] { command with { Login = "    " } };
        yield return new object[] { command with { Login = "123456789012345678901" } };
        yield return new object[] { command with { Password = string.Empty } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandInvalid(SignInCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }
}
