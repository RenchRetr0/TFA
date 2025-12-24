using System.Security.Cryptography;
using FluentAssertions;
using TFA.Domain.Authentication;
using Xunit.Abstractions;

namespace TFA.Domain.Tests.Authentication;

public class AesSymmetricEncryptorDecryptorShould
{
    private readonly AesSymmetricEncryptorDecryptor sut = new();
    private readonly byte[] key = RandomNumberGenerator.GetBytes(32);
    private readonly ITestOutputHelper testOutputHelper;

    public AesSymmetricEncryptorDecryptorShould(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task ReturnMeaningfulEncryptedString()
    {
        var actual = await sut.Encrypt("Hello world!", key, CancellationToken.None);

        actual.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DecryptEncryptedString_WhenKeyIsSame()
    {
        var encrypted = await sut.Encrypt("Hello world!", key, CancellationToken.None);
        var decrypted = await sut.Decrypt(encrypted, key, CancellationToken.None);

        decrypted.Should().Be("Hello world!");
    }

    [Fact]
    public async Task ThrowException_WhenDecryptingWithDifferentKey()
    {
        var encrypted = await sut.Encrypt("Hello world!", key, CancellationToken.None);
        await sut.Invoking(s => s.Decrypt(encrypted, RandomNumberGenerator.GetBytes(32), CancellationToken.None))
            .Should().ThrowAsync<CryptographicException>();
    }

    // [Fact]
    // public void GiveMeBase64Key()
    // {
    //     testOutputHelper.WriteLine(Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));
    // }
}
