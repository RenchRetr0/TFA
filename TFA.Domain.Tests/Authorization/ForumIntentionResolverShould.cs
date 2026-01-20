using FluentAssertions;
using Moq;
using TFA.Domain.Authentication;
using TFA.Domain.UseCase.CreateForum;

namespace TFA.Domain.Tests.Authorization;

public class ForumIntentionResolverShould
{
    private readonly ForumIntentionResolver sut = new();

    [Fact]
    public void ReturnFalse_WhenIntentionNotEnum()
    {
        var intention = (ForumIntention)(-1);
        sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingForumCreateIntention_AndUserIsGuest()
    {
        sut.IsAllowed(User.Guest, ForumIntention.Create).Should().BeFalse();
    }

    [Fact]
    public void ReturnTrue_WhenCheckingForumCreateIntention_AndUserIsAuthenticated()
    {
        sut.IsAllowed(
            new User(Guid.Parse("629fa128-1108-47e1-8e47-d7f2b7f2df83"), Guid.Empty), ForumIntention.Create)
            .Should().BeTrue();
    }
}
