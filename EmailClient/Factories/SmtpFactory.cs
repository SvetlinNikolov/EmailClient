namespace EmailClient.Factories;

using EmailClient.Services;
using EmailClient.Factories.Contracts;
using EmailClient.Services.Contracts;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient Create(string host, int port, string username, string password)
        => new SmtpClient(host, port, username, password);
}
