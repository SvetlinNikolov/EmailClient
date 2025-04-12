namespace EmailClient.Services;

using EmailClient.Auth;
using EmailClient.Domain.Results;
using EmailClient.Factories.Contracts;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Login;

public class LoginService(
    IImapClientFactory imapClientFactory,
    ICookieAuthService cookieAuthService) : ILoginService
{
    public async Task<Result> LoginAsync(LoginViewModel loginViewModel)
    {
        var client = imapClientFactory.Create(
            loginViewModel.ImapLogin.ImapServer,
            loginViewModel.ImapLogin.ImapPort,
            loginViewModel.ImapLogin.ImapUsername,
            loginViewModel.ImapLogin.ImapPassword
        );

        var connectAndLogin = await client.ConnectAndLoginAsync();
        if (!connectAndLogin.IsSuccess) return connectAndLogin;

        var cookie = MapLoginVmToLoginCookie(loginViewModel);

        return cookieAuthService.SaveLoginCookie(cookie);
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
