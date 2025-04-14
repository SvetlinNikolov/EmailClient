namespace EmailClient.Services.Services.Contracts;

using EmailClient.Imap.Client.Contracts;
using EmailClient.Services.Auth;
using EmailService.Domain.Results;

public interface IIMapSessionService
{
    Task<Result> GetOrCreateAsync(string sessionId, LoginCookie loginData);
    public void RegisterClient(string sessionId, IImapClient client);
    public void EndSession(string sessionId);

}
