namespace TFA.Domain.Authentication;

public class User : IIdentity
{
    public User(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }

    public static User Guest => new(Guid.Empty);
}