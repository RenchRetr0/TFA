using FluentAssertions;
using TFA.Domain.UseCase.CreateTopic;

namespace TFA.Domain.Tests;

public class CreateTopicCommandValidatorShould
{
    private readonly CreateTopicCommandValidator sut;

    public CreateTopicCommandValidatorShould()
    {
        sut = new CreateTopicCommandValidator();
    }

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var command = new CreateTopicCommand(Guid.Parse("eff3885b-cb2c-436f-a5ed-9676c58e6be6"), "Hello");
        var actual = sut.Validate(command);
        actual.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateTopicCommand(Guid.Parse("434073c0-e9af-4c65-9839-e86ee6f777b2"), "Hello");
        yield return new object[]{ validCommand with { ForumId = Guid.Empty }};
        yield return new object[]{ validCommand with { Title = string.Empty }};
        yield return new object[]{ validCommand with { Title = "    " }};
        yield return new object[]{ validCommand with { Title = string.Join("a", Enumerable.Range(0, 100)) }};
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandIsInvalid(CreateTopicCommand command)
    {
        var actual = sut.Validate(command);
        actual.IsValid.Should().BeFalse();
    }
}