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

    public SignInUseCaseShould()
    {
        var validator = new Mock<IValidator<SignInCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<SignInCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var storage = new Mock<ISignInStorage>();
        findUserSetup = storage.Setup(s => s.FindUser(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        var passwordManger = new Mock<IPasswordManager>();
        comparePasswordsSetup = passwordManger.Setup(
            m => m.ComparePasswords(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()));

        var encryptor = new Mock<ISymmetricEncryptor>();
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
    public async Task ReturnToken()
    {
        var userId = Guid.Parse("c26e9bd4-b96a-4dbc-9441-a3c7c218c00e");
        var recognizedUser = new RecognizedUser
        {
            UserId = userId,
            Salt = [1],
            PasswordHash = [2]
        };

        findUserSetup.ReturnsAsync(recognizedUser);
        comparePasswordsSetup.Returns(true);
        encryptSetup.ReturnsAsync("token");

        var (identity, token) = await sut.Execute(new SignInCommand("Test", "test228"), CancellationToken.None);
        identity.UserId.Should().Be(userId);
        token.Should().Be("token");
    }
}
