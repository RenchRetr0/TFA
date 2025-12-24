namespace TFA.Domain.Authentication;

public class IdentityProvider : IIdentityProvider
{
    public required IIdentity Current { get; set; }
}