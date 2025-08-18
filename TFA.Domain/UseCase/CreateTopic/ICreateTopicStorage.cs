using TFA.Domain.Models;

namespace TFA.Domain.UseCase.CreateTopic;

public interface ICreateTopicStorage
{
    Task<Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken);
}