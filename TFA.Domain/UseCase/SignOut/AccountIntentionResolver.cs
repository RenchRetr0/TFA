using TFA.Domain.Authentication;
using TFA.Domain.Authorization;

namespace TFA.Domain.UseCase.SignOut;

internal class AccountIntentionResolver : IIntentionResolver<AccountIntention>
{
    public bool IsAllowed(IIdentity subject, AccountIntention intention) => intention switch
    {
        AccountIntention.SignOut => subject.IsAuthentication(),
        _ => false,
    };
}
