namespace EmailClient.Factories.Contracts;

using EmailClient.Services.Contracts;

public interface IImapClientFactory
{
    IImapClient Create(string host, int port, string username, string password);
}
