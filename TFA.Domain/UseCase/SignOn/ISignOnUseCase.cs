using TFA.Domain.Authentication;

namespace TFA.Domain.UseCase.SignOn;

public interface ISignOnUseCase
{
    Task<IIdentity> Execute(SignOnCommand command, CancellationToken cancellationToken);
}
