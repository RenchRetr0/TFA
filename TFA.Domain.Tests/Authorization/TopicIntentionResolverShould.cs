using FluentAssertions;
using Moq;
using TFA.Domain.Authentication;
using TFA.Domain.UseCase.CreateTopic;

namespace TFA.Domain.Tests.Authorization;

public class TopicIntentionResolverShould
{
    private readonly TopicIntentionResolver sut = new();

    [Fact]
    public void ReturnFalse_WhenIntentionNotEnum()
    {
        var intention = (TopicIntention)(-1);
        sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingTopicCreateIntention_AndUserIsGuest()
    {
        sut.IsAllowed(User.Guest, TopicIntention.Create).Should().BeFalse();
    }

    [Fact]
    public void ReturnTrue_WhenCheckingTopicCreateIntention_AndUserIsAuthenticated()
    {
        sut.IsAllowed(
            new User(Guid.Parse("629fa128-1108-47e1-8e47-d7f2b7f2df83"), Guid.Empty), TopicIntention.Create)
            .Should().BeTrue();
    }
}
