using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;
using TFA.Domain.UseCase.SignIn;

namespace TFA.Domain.Tests.SignIn;

public class SignInUseCaseShould
{
    private readonly SignInUseCase sut;
    private readonly ISetup<ISignInStorage, Task<RecognizedUser?>> findUserSetup;
    private readonly ISetup<IPasswordManager, bool> comparePasswordsSetup;
    private readonly ISetup<ISymmetricEncryptor, Task<string>> encryptSetup;
    private readonly ISetup<ISignInStorage, Task<Guid>> createSessionSetup;
    private readonly Mock<ISignInStorage> storage;
    private readonly Mock<ISymmetricEncryptor> encryptor;

    public SignInUseCaseShould()
    {
        var validator = new Mock<IValidator<SignInCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<SignInCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        storage = new Mock<ISignInStorage>();
        findUserSetup = storage.Setup(s => s.FindUser(It.IsAny<string>(), It.IsAny<CancellationToken>()));
        createSessionSetup = storage.Setup(s => s.CreateSession(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()));

        var passwordManger = new Mock<IPasswordManager>();
        comparePasswordsSetup = passwordManger.Setup(
            m => m.ComparePasswords(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()));

        encryptor = new Mock<ISymmetricEncryptor>();
        encryptSetup = encryptor.Setup(e => e.Encrypt(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));

        var configuration = new Mock<IOptions<AuthenticationConfiguration>>();
        configuration
            .Setup(c => c.Value)
            .Returns(new AuthenticationConfiguration()
            {
                Base64Key = "OP9l1Lta1Aw9V9OAadOgM4vRwdIy/1K2BKh18FJ8nmQ="
            });

        sut = new SignInUseCase(
            validator.Object,
            storage.Object,
            passwordManger.Object,
            encryptor.Object,
            configuration.Object
        );
    }

    [Fact]
    public async Task ThrowValidationException_WhenUserNotFound()
    {
        findUserSetup.ReturnsAsync(() => null);
        (await sut.Invoking(s => s.Execute(new SignInCommand("Test", "test228"), CancellationToken.None))
                .Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.PropertyName == "Login");
    }

    [Fact]
    public async Task ThrowValidationException_WhenPasswordDoesntMatch()
    {
        var recognizedUser = new RecognizedUser
        {
            UserId = Guid.NewGuid(),
            Salt = [0x01, 0x02, 0x03, 0x04],
            PasswordHash = [0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C]
        };

        findUserSetup.ReturnsAsync(recognizedUser);
        comparePasswordsSetup.Returns(false);

        (await sut.Invoking(s => s.Execute(new SignInCommand("Test", "test228"), CancellationToken.None))
                .Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task CreateSession_WhenPasswordMatches()
    {
        var userId = Guid.Parse("e8e9507e-1b4b-43d8-a784-73aa1ab453cc");
        var sessionId = Guid.Parse("15d27d34-8a08-4ec1-812c-c4ae6a579615");
        findUserSetup.ReturnsAsync(new RecognizedUser
        {
            UserId = userId,
            PasswordHash = [2],
            Salt = [1],
        });
        comparePasswordsSetup.Returns(true);
        createSessionSetup.ReturnsAsync(sessionId);

        await sut.Execute(new SignInCommand("test", "test228"), CancellationToken.None);
        storage.Verify(s => s.CreateSession(userId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReturnTokenAndIdentity()
    {
        var userId = Guid.Parse("c26e9bd4-b96a-4dbc-9441-a3c7c218c00e");
        var sessionId = Guid.Parse("15d27d34-8a08-4ec1-812c-c4ae6a579615");
        var recognizedUser = new RecognizedUser
        {
            UserId = userId,
            Salt = [1],
            PasswordHash = [2]
        };

        findUserSetup.ReturnsAsync(recognizedUser);
        createSessionSetup.ReturnsAsync(sessionId);
        comparePasswordsSetup.Returns(true);
        encryptSetup.ReturnsAsync("token");

        var (identity, token) = await sut.Execute(new SignInCommand("Test", "test228"), CancellationToken.None);
        identity.UserId.Should().Be(userId);
        identity.UserId.Should().Be(userId);
        identity.SessionId.Should().Be(sessionId);
        token.Should().Be("token");
    }

    [Fact]
    public async Task EncryptSessionIdIntoToken()
    {
        var userId = Guid.Parse("e8e9507e-1b4b-43d8-a784-73aa1ab453cc");
        var sessionId = Guid.Parse("15d27d34-8a08-4ec1-812c-c4ae6a579615");
        findUserSetup.ReturnsAsync(new RecognizedUser
        {
            UserId = userId,
            PasswordHash = [2],
            Salt = [1],
        });
        comparePasswordsSetup.Returns(true);
        createSessionSetup.ReturnsAsync(sessionId);

        await sut.Execute(new SignInCommand("test", "test228"), CancellationToken.None);
        encryptor.Verify(e => e
            .Encrypt(sessionId.ToString(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));
    }
}
