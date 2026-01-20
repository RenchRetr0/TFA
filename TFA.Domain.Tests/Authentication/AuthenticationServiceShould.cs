using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class AuthenticationServiceShould
{
    private readonly AuthenticationService sut;
    private readonly ISetup<ISymmetricDecryptor, Task<string>> setupDecryptor;
    private readonly ISetup<IAuthenticationStorage, Task<Session?>> findSessionSetup;

    public AuthenticationServiceShould()
    {
        var decryptor = new Mock<ISymmetricDecryptor>();
        setupDecryptor = decryptor.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));

        var storage = new Mock<IAuthenticationStorage>();
        findSessionSetup = storage.Setup(s => s.FindSession(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

        var options = new Mock<IOptions<AuthenticationConfiguration>>();
        options
            .Setup(o => o.Value)
            .Returns(new AuthenticationConfiguration
            {
                Base64Key = "OP9l1Lta1Aw9V9OAadOgM4vRwdIy/1K2BKh18FJ8nmQ="
            });

        sut = new AuthenticationService(
            decryptor.Object,
            storage.Object,
            NullLogger<AuthenticationService>.Instance,
            options.Object);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenCannotBeDecrypted()
    {
        setupDecryptor.Throws<CryptographicException>();
        var actual = await sut.Authenticate("bad-token", CancellationToken.None);

        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenIsInvalid()
    {
        setupDecryptor.ReturnsAsync("not-a-guid");
        var actual = await sut.Authenticate("bad-token", CancellationToken.None);

        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenSessionNotFound()
    {
        setupDecryptor.ReturnsAsync("0755536a-c47e-4cbc-b701-51d9b5424f8f");
        findSessionSetup.ReturnsAsync(new Session
        {
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1)
        });

        var actual = await sut.Authenticate("good-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenSessionIsExpired()
    {
        setupDecryptor.ReturnsAsync("0755536a-c47e-4cbc-b701-51d9b5424f8f");
        findSessionSetup.ReturnsAsync(() => null);

        var actual = await sut.Authenticate("good-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnIdentity_WhenSessionIsValid()
    {
        var sessionId = Guid.Parse("baf94868-2cfd-4fb3-9836-eb320452c09c");
        var userId = Guid.Parse("c4dff441-271f-47bd-b1fa-d33d03ef4eca");

        setupDecryptor.ReturnsAsync("baf94868-2cfd-4fb3-9836-eb320452c09c");
        findSessionSetup.ReturnsAsync(new Session
        {
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1)
        });

        var actual = await sut.Authenticate("good-token", CancellationToken.None);

        actual.Should().BeEquivalentTo(new User(userId, sessionId));
    }
}
