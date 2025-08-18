using FluentValidation;
using TFA.Domain.Models;
using TFA.Domain.UseCase.GetForums;

namespace TFA.Domain.UseCase.GetTopics;

internal class GetTopicsUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetTopicsStorage storage;
    private readonly IGetForumsStorage getForumsStorage;

    public GetTopicsUseCase(
        IValidator<GetTopicsQuery> validator,
        IGetTopicsStorage storage,
        IGetForumsStorage getForumsStorage
    )
    {
        this.validator = validator;
        this.storage = storage;
        this.getForumsStorage = getForumsStorage;
    }

    public async Task<(IEnumerable<Topic> resources, int totalCount)> Execute(
        GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);
        await getForumsStorage.ThrowIfForumNotFound(query.ForumId, cancellationToken);
        return await storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}