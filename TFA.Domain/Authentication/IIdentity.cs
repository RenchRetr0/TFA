namespace TFA.Domain.Authentication;

public interface IIdentity
{
    Guid UserId { get; }
}

internal static class IdentityExceptions
{
    public static bool IsAuthentication(this IIdentity identity) => identity.UserId != Guid.Empty;
}