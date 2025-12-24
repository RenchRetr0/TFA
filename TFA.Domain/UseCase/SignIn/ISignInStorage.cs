namespace TFA.Domain.UseCase.SignIn;

public interface ISignInStorage
{
    Task<RecognizedUser?> FindUser(string login, CancellationToken cancellationToken);
}
