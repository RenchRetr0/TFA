namespace TFA.Domain.UseCase.SignIn;

public class RecognizedUser
{
    public Guid UserId { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] PasswordHash { get; set; }
}
