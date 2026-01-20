using TFA.Domain.UseCase.SignOut;

namespace TFA.Storage.Storages;

internal class SignOutStorage : ISignOutStorage
{
    public Task RemoveSession(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
