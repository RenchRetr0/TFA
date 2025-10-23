using TFA.Domain.Models;

namespace TFA.Domain.UseCase.CreateForum;

public interface ICreateForumUseCase
{
    Task<Forum> Execute(CreateForumCommand command, CancellationToken cancellationToken);
}
