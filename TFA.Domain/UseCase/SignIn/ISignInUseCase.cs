using TFA.Domain.Authentication;

namespace TFA.Domain.UseCase.SignIn;

public interface ISignInUseCase
{
    Task<(IIdentity identity, string token)> Execute(SignInCommand command, CancellationToken cancellationToken);
}
