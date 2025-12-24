using FluentValidation;
using Microsoft.Extensions.Options;
using TFA.Domain.Authentication;

namespace TFA.Domain.UseCase.SignIn;

internal class SignInUseCase : ISignInUseCase
{
    private IValidator<SignInCommand> validator;
    private ISignInStorage storage;
    private IPasswordManager passwordManager;
    private ISymmetricEncryptor encryptor;
    private AuthenticationConfiguration configuration;

    public SignInUseCase(
        IValidator<SignInCommand> validator,
        ISignInStorage storage,
        IPasswordManager passwordManager,
        ISymmetricEncryptor encryptor,
        IOptions<AuthenticationConfiguration> options
    )
    {
        this.validator = validator;
        this.storage = storage;
        this.passwordManager = passwordManager;
        this.encryptor = encryptor;
        configuration = options.Value;
    }
    public async Task<(IIdentity identity, string token)> Execute(SignInCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var recognizedUser = await storage.FindUser(command.Login, cancellationToken);
        if (recognizedUser is null) throw new Exception();

        var passwordMatches = passwordManager.ComparePasswords(
            command.Password, recognizedUser.Salt, recognizedUser.PasswordHash);
        if (!passwordMatches) throw new Exception();

        var token = await encryptor.Encrypt(
            recognizedUser.UserId.ToString(), configuration.Key, cancellationToken);
        return (new User(recognizedUser.UserId), token);
    }
}
