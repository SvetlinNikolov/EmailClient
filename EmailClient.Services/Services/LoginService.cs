namespace EmailClient.Services.Services;

using EmailClient.Services.Auth;
using EmailClient.Services.Services.Contracts;
using EmailClient.ViewModels.ViewModels.Login;
using EmailService.Domain.Errors;
using EmailService.Domain.Results;
using Microsoft.AspNetCore.Http;

public class LoginService(
    ICookieAuthService cookieAuthService,
    IHttpContextAccessor httpContextAccessor,
    IIMapSessionService imapSessionService) : ILoginService
{
    public async Task<Result> LoginAsync(LoginViewModel loginViewModel)
    {
        var isLoggedInResult = cookieAuthService.IsLoggedIn();
        if (isLoggedInResult.IsSuccess)
        {
            return Result.Failure(CookieErrors.AlreadyLoggedIn());
        }

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

    public Result Logout()
    {
        try
        {
            cookieAuthService.ClearLoginCookie();
            var sessionId = httpContextAccessor.HttpContext!.Session.Id;
            imapSessionService.EndSession(sessionId);

            return Result.Success();
        }
        catch
        {
            // log
            return Result.Failure(CookieErrors.CookieDeserializationFailed());
        }
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
