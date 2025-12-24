namespace TFA.API.Authentication;

public interface IAuthTokenStorage
{
    bool TryExtract(HttpContext httpContent, out string token);
    void Store(HttpContext httpContext, string token);
}
