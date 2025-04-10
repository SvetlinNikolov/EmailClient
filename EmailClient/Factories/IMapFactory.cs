namespace EmailClient.Factories;

using EmailClient.Services;
using EmailClient.Factories.Contracts;
using EmailClient.Services.Contracts;

public class ImapClientFactory : IImapClientFactory
{
    public IImapClient Create(string host, int port, string username, string password)
        => new ImapClient(host, port, username, password);
}
