using TFA.Domain.Models;

namespace TFA.Domain.UseCase.CreateTopic;

public interface ICreateTopicUseCase
{
    Task<Topic> Execute(Guid forumId, string Title, CancellationToken cancellationToken);
}