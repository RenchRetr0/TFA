using TFA.Domain.Models;

namespace TFA.Domain.UseCase.GetForums;

public interface IGetForumsStorage
{
    Task<IEnumerable<Forum>> GetForums(CancellationToken cancellationToken);
}