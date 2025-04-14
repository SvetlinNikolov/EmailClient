namespace EmailClient.Services.Factories;

using EmailClient.Imap.Client;
using EmailClient.Imap.Client.Contracts;
using EmailClient.Services.Factories.Contracts;

public class ImapClientFactory : IImapClientFactory
{
    public IImapClient Create(string host, int port, string username, string password)
        => new ImapClient(host, port, username, password);
}
