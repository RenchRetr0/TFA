using TFA.Domain.Models;

namespace TFA.Domain.UseCase.CreateForum;

public interface ICreateForumStorage
{
    public Task<Forum> Create(string title, CancellationToken cancellationToken);
}
