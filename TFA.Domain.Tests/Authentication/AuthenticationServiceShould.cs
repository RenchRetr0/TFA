using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class AuthenticationServiceShould
{
    private readonly AuthenticationService sut;
    private readonly ISetup<ISymmetricDecryptor, Task<string>> setupDecryptor;

    public AuthenticationServiceShould()
    {
        var decryptor = new Mock<ISymmetricDecryptor>();
        setupDecryptor = decryptor.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));

        var options = new Mock<IOptions<AuthenticationConfiguration>>();
        options
            .Setup(o => o.Value)
            .Returns(new AuthenticationConfiguration
            {
                Base64Key = "OP9l1Lta1Aw9V9OAadOgM4vRwdIy/1K2BKh18FJ8nmQ="
            });

        sut = new AuthenticationService(decryptor.Object, options.Object);
    }

    [Fact]
    public async Task ExtractAndReturnIdentityFromToken()
    {
        setupDecryptor.ReturnsAsync("baf94868-2cfd-4fb3-9836-eb320452c09c");

        var actual = await sut.Authenticate("some token", CancellationToken.None);

        actual.Should().BeEquivalentTo(new User(Guid.Parse("baf94868-2cfd-4fb3-9836-eb320452c09c")));
    }
}
