using TFA.Domain.Models;

namespace TFA.Domain.UseCase.GetForums;

public interface IGetForumsUseCase
{
    Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken);
}