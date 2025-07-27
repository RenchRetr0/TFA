namespace TFA.Domain.Authentication;

public class IdentityProvider : IIdentityProvider
{
    public IIdentity Current => new User(Guid.Parse("db9615bb-3b4b-47e2-a27e-a2fd6b682654"));
}