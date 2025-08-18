namespace TFA.Domain.UseCase.GetTopics;

public record GetTopicsQuery(Guid ForumId, int Skip, int Take);
