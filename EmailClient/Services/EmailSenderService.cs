namespace EmailClient.Services; 

using EmailClient.Auth;
using EmailClient.Domain.Results;
using EmailClient.Factories.Contracts;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email.Requests;


public class EmailSenderService(ICookieAuthService cookieAuthService, ISmtpClientFactory smtpClientFactory) : IEmailSenderService
{
    public async Task<Result> SendEmailAsync(SendEmailRequest request)
    {
        var loginDataResult = cookieAuthService.GetLoginCookie();
        if (!loginDataResult.IsSuccess) return loginDataResult;

        var loginData = loginDataResult.GetData<LoginCookie>();
        var smtpClient = smtpClientFactory.Create(loginData.SmtpServer, loginData.SmtpPort, loginData.SmtpUsername, loginData.SmtpPassword);

        var sendEmailResult = await smtpClient.SendEmailAsync(loginData.SmtpUsername, request.To, request.Subject, request.Body);

        return sendEmailResult;
    }
}
