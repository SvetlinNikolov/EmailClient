namespace EmailClient.Factories.Contracts;

using EmailClient.Services.Contracts;

public interface ISmtpClientFactory
{
    ISmtpClient Create(string host, int port, string username, string password);
}
