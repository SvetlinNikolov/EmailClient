namespace EmailClient.Services.Contracts;

using EmailClient.Auth;
using EmailClient.Domain.Results;

public interface IIMapSessionService
{
    Task<Result> GetOrCreateAsync(string sessionId, LoginCookie loginData);
    public void RegisterClient(string sessionId, IImapClient client);
    public void EndSession(string sessionId);
    
}
