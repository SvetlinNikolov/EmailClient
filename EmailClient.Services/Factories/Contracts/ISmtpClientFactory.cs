namespace EmailClient.Services.Factories.Contracts;

using EmailClient.Smtp.Client.Contracts;

public interface ISmtpClientFactory
{
    ISmtpClient Create(string host, int port, string username, string password);
}
