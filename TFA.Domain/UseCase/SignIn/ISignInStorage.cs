namespace TFA.Domain.UseCase.SignIn;

public interface ISignInStorage
{
    Task<RecognizedUser?> FindUser(string login, CancellationToken cancellationToken);

    Task<Guid> CreateSession(Guid UserId, DateTimeOffset expirationMoment, CancellationToken cancellationToken);
}
