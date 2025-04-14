using EmailClient.ViewModels.ViewModels.Email.Requests;
using EmailService.Domain.Results;

namespace EmailClient.Services.Services.Contracts;

public interface IEmailSenderService
{
    Task<Result> SendEmailAsync(SendEmailRequest request);
}
