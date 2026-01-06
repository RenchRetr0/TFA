using System.Net;
using FluentAssertions;
using Moq;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCase.CreateForum;

namespace TFA.Domain.Tests.Authorization;

public class IIntentionManagerShould
{
    [Fact]
    public void ReturnFalse_WhenNoMatchingResolver()
    {
        var sut = new IntentionManager(
            [
                new Mock<IIntentionResolver<DomainErrorCode>>().Object,
                new Mock<IIntentionResolver<HttpStatusCode>>().Object,
            ],
            new Mock<IIdentityProvider>().Object);

        sut.IsAllowed(ForumIntention.Create).Should().BeFalse();
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void ReturnMatchingResolverResult(bool expectedResolverResult, bool expected)
    {
        var resolver = new Mock<IIntentionResolver<ForumIntention>>();
        resolver
            .Setup(r => r.IsAllowed(It.IsAny<IIdentity>(), It.IsAny<ForumIntention>()))
            .Returns(expectedResolverResult);

        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider
            .Setup(p => p.Current)
            .Returns(new User(Guid.Parse("c0b4eb28-cdfd-4a87-b8e6-ce1882f00131")));

        var sut = new IntentionManager([resolver.Object], identityProvider.Object);

        sut.IsAllowed(ForumIntention.Create).Should().Be(expected);
    }
}
