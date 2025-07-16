namespace TFA.Domain.UseCase.CreateTopic;

public record CreateTopicCommand(Guid ForumId, string Title);