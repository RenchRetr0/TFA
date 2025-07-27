namespace TFA.Domain.Authentication;

internal interface IIdentityProvider
{
    IIdentity Current { get; }
}