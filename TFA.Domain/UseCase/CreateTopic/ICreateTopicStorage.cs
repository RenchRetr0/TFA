using TFA.Domain.Models;

namespace TFA.Domain.UseCase.CreateTopic;

public interface ICreateTopicStorage
{
    Task<bool> ForumExists(Guid forumId, CancellationToken cancellationToken);
    Task<Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken);
}