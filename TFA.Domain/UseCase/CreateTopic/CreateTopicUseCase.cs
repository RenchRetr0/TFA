using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using Topic = TFA.Domain.Models.Topic;

namespace TFA.Domain.UseCase.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IIntentionManager intentionManager;
    private readonly IIdentityProvider identityProvider;
    private readonly ICreateTopicStorage storage;

    public CreateTopicUseCase(
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        ICreateTopicStorage storage
    )
    {
        this.intentionManager = intentionManager;
        this.storage = storage;
        this.identityProvider = identityProvider;
    }

    public async Task<Topic> Execute(Guid forumId, string title, CancellationToken cancellationToken)
    {
        intentionManager.ThrowIfForbidden(TopicIntention.Create);
        
        var forumExist = await storage.ForumExists(forumId, cancellationToken);
        if(!forumExist)
        {
            throw new ForumNotFoundException(forumId);
        }

        return await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}