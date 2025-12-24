namespace TFA.Domain.UseCase.SignOn;

public interface ISignOnStorage
{
    Task<Guid> CreateUser(string login, byte[] salt, byte[] hash, CancellationToken cancellationToken);
}
