namespace EmailClient.Services.Contracts;

using EmailClient.Domain.Results;
using EmailClient.ViewModels.Email.Requests;

public interface IEmailSenderService
{
    Task<Result> SendEmailAsync(SendEmailRequest request);
}
