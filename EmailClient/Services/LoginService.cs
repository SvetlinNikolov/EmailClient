namespace EmailClient.Services;

using EmailClient.Auth;
using EmailClient.Domain.Results;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Login;

public class LoginService(
    ICookieAuthService cookieAuthService,
    IHttpContextAccessor httpContextAccessor,
    IIMapSessionService imapSessionService) : ILoginService
{
    public async Task<Result> LoginAsync(LoginViewModel loginViewModel)
    {
        var cookieResult = cookieAuthService.SaveLoginCookie(MapLoginVmToLoginCookie(loginViewModel));
        if (!cookieResult.IsSuccess)
        {
            return cookieResult;
        }

        var sessionId = httpContextAccessor.HttpContext!.Session.Id;
        var sessionResult = await imapSessionService.GetOrCreateAsync(sessionId, cookieResult.GetData<LoginCookie>());
        if (!sessionResult.IsSuccess)
        {
            return sessionResult;
        }

        return Result.Success();
    }

    private static LoginCookie MapLoginVmToLoginCookie(LoginViewModel loginViewModel)
    {
        return new LoginCookie
        {
            ImapServer = loginViewModel.ImapLogin.ImapServer,
            ImapPort = loginViewModel.ImapLogin.ImapPort,
            ImapUsername = loginViewModel.ImapLogin.ImapUsername,
            ImapPassword = loginViewModel.ImapLogin.ImapPassword,

            SmtpServer = loginViewModel.SmtpLogin.SmtpServer,
            SmtpPort = loginViewModel.SmtpLogin.SmtpPort,
            SmtpUsername = loginViewModel.SmtpLogin.SmtpUsername,
            SmtpPassword = loginViewModel.SmtpLogin.SmtpPassword
        };
    }
}
