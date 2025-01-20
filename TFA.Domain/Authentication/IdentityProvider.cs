namespace TFA.Domain.Authentication;

public class IdentityProvider : IIdentityProvider
{
    public IIdentity Current => new User(Guid.Parse("9fc3581b-10b0-45c3-886b-4084670ec429"));
}