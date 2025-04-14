using EmailService.Domain.Results;

namespace EmailClient.Services.Auth;

public interface ICookieAuthService
{
    Result SaveLoginCookie(LoginCookie payload);
    Result GetLoginCookie();
    void ClearLoginCookie();
    Result IsLoggedIn();
}
