namespace EmailClient.Services.Factories;

using EmailClient.Services.Factories.Contracts;
using EmailClient.Smtp.Client;
using EmailClient.Smtp.Client.Contracts;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient Create(string host, int port, string username, string password)
        => new SmtpClient(host, port, username, password);
}
