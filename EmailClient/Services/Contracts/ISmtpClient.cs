using EmailClient.Domain.Results;

namespace EmailClient.Services.Contracts;

public interface ISmtpClient
{
    Task<Result> SendEmailAsync(string from, string to, string subject, string body);
}
