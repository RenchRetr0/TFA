namespace TFA.Domain.Authorization;

public class IntentionManagerExtension : Exception
{
    public IntentionManagerExtension() : base("Action is not allowed")
    {}
}