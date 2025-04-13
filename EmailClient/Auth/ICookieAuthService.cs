namespace EmailClient.Auth;

using EmailClient.Domain.Results;

public interface ICookieAuthService
{
    Result SaveLoginCookie(LoginCookie payload);
    Result GetLoginCookie();
    void ClearLoginCookie();
    Result IsLoggedIn();
}
