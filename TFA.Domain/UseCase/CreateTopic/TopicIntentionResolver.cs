using TFA.Domain.Authentication;
using TFA.Domain.Authorization;

namespace TFA.Domain.UseCase.CreateTopic;

public class TopicIntentionResolver : IIntentionResolver<TopicIntention>
{
    public bool IsAllowed(IIdentity subject, TopicIntention intention) => intention switch
    {
        TopicIntention.Create => subject.IsAuthentication(),
        _ => false
    };
}