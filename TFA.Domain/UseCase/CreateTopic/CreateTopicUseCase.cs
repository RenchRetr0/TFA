using FluentValidation;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.UseCase.GetForums;
using Topic = TFA.Domain.Models.Topic;

namespace TFA.Domain.UseCase.CreateTopic;

internal class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IValidator<CreateTopicCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly IIdentityProvider identityProvider;
    private readonly IGetForumsStorage getForumsStorage;
    private readonly ICreateTopicStorage storage;

    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        IGetForumsStorage getForumsStorage,
        ICreateTopicStorage storage
    )
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.storage = storage;
        this.identityProvider = identityProvider;
        this.getForumsStorage = getForumsStorage;
    }

    public async Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var (forumId, title) = command;
        intentionManager.ThrowIfForbidden(TopicIntention.Create);

        await getForumsStorage.ThrowIfForumNotFound(command.ForumId, cancellationToken);

        return await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}