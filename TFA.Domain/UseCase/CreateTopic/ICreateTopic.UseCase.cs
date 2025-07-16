using TFA.Domain.Models;

namespace TFA.Domain.UseCase.CreateTopic;

public interface ICreateTopicUseCase
{
    Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken);
}