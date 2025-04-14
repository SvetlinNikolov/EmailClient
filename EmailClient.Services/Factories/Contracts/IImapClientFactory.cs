namespace EmailClient.Services.Factories.Contracts;

using EmailClient.Imap.Client.Contracts;

public interface IImapClientFactory
{
    IImapClient Create(string host, int port, string username, string password);
}
