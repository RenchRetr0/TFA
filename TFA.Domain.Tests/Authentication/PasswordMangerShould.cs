using FluentAssertions;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class PasswordMangerShould
{
    private readonly PasswordManager sut = new();
    private static readonly byte[] emptySalt = Enumerable.Repeat((byte)0, 100).ToArray();
    private static readonly byte[] emptyHash = Enumerable.Repeat((byte)0, 32).ToArray();

    [Theory]
    [InlineData("password")]
    [InlineData("world228")]
    public void GenerateMeaningfulSaltAndHash(string password)
    {
        var (salt, hash) = sut.GeneratePasswordParts(password);
        salt.Should().HaveCount(100).And.NotBeEquivalentTo(emptySalt);
        hash.Should().HaveCount(32).And.NotBeEquivalentTo(emptyHash);
    }

    [Fact]
    public void ReturnTrue_WhenPasswordMatch()
    {
        var password = "world228";
        var (salt, hash) = sut.GeneratePasswordParts(password);
        sut.ComparePasswords(password, salt, hash).Should().BeTrue();
    }

    [Fact]
    public void ReturnFalse_WhenPasswordDoesntMatch()
    {

        var passwordOne = "world228";
        var passwordTow = "notWorld228";
        var (salt, hash) = sut.GeneratePasswordParts(passwordOne);
        sut.ComparePasswords(passwordTow, salt, hash).Should().BeFalse();
    }
}
