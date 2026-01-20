namespace TFA.Domain.UseCase.SignOut;

public interface ISignOutUseCase
{
    Task Execute(SignOutComand comand, CancellationToken cancellationToken);
}
