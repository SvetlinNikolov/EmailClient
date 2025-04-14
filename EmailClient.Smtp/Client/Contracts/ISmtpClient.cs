using EmailService.Domain.Results;

namespace EmailClient.Smtp.Client.Contracts;

public interface ISmtpClient
{
    Task<Result> SendEmailAsync(string from, string to, string subject, string body);
}
