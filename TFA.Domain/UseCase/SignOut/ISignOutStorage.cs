namespace TFA.Domain.UseCase.SignOut;

public interface ISignOutStorage
{
    Task RemoveSession(Guid sessionId, CancellationToken cancellationToken);
}
