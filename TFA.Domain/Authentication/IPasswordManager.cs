namespace TFA.Domain.Authentication;

public interface IPasswordManager
{
    (byte[] Salt, byte[] Hash) GeneratePasswordParts(string password);

    bool ComparePasswords(string password, byte[] salt, byte[] hash);
}
