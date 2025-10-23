using TFA.Domain.Authentication;
using TFA.Domain.Authorization;

namespace TFA.Domain.UseCase.CreateForum;

internal class ForumIntentionResolver : IIntentionResolver<ForumIntention>
{
    public bool IsAllowed(IIdentity subject, ForumIntention intention) => intention switch
    {
        ForumIntention.Create => subject.IsAuthentication(),
        _ => false
    };
}
