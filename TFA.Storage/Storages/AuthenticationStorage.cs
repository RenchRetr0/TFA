using TFA.Domain.Authentication;

namespace TFA.Storage.Storages;

internal class AuthenticationStorage : IAuthenticationStorage
{
    public Task<Session?> FindSession(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
