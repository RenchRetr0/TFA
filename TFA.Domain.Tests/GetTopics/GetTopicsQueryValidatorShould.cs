using FluentAssertions;
using TFA.Domain.UseCase.GetTopics;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsQueryValidatorShould
{
    private readonly GetTopicsQueryValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenQueryIsValid()
    {
        var query = new GetTopicsQuery(Guid.Parse("2f679127-d670-41b6-8d17-c155b50c9895"), 10, 5);
        sut.Validate(query).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidQuery()
    {
        var query = new GetTopicsQuery(Guid.Parse("2f679127-d670-41b6-8d17-c155b50c9895"), 10, 5);
        yield return new object[] { query with { ForumId = Guid.Empty } };
        yield return new object[] { query with { Skip = -40 } };
        yield return new object[] { query with { Take = -4 } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidQuery))]
    public void ReturnFailure_WhenQueryIsInvalid(GetTopicsQuery query)
    {
        sut.Validate(query).IsValid.Should().BeFalse();
    }
}
