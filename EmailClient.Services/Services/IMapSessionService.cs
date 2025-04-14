namespace EmailClient.Services.Services;

using EmailClient.Imap.Client.Contracts;
using EmailClient.Imap.Errors;
using EmailClient.Services.Auth;
using EmailClient.Services.Factories.Contracts;
using EmailClient.Services.Services.Contracts;
using EmailService.Domain.Cache;
using EmailService.Domain.Results;

public class IMapSessionService(ICacheService cache, IImapClientFactory imapFactory) : IIMapSessionService
{
    public async Task<Result> GetOrCreateAsync(string sessionId, LoginCookie loginData)
    {
        if (cache.TryGet<IImapClient>(sessionId, out var cached) && cached is IImapClient client && client.IsConnected())
        {
            return Result.Success(client);
        }

        if (cached is IDisposable disposable)
        {
            disposable.Dispose();
            cache.Remove(sessionId);
        }


        var newClient = imapFactory.Create(loginData.ImapServer, loginData.ImapPort, loginData.ImapUsername, loginData.ImapPassword);
        var loginResult = await newClient.ConnectAndLoginAsync();
        if (!loginResult.IsSuccess)
        {
            newClient.Dispose();
            return Result.Failure(ImapErrors.ImapLoginFailed());
        }

        cache.Set(sessionId, newClient, CacheConfig.SessionTtl);
        return Result.Success(newClient);
    }

    public void RegisterClient(string sessionId, IImapClient client)
    {
        cache.Set(sessionId, client, CacheConfig.SessionTtl);
    }

    public void EndSession(string sessionId)
    {
        if (cache.TryGet<IImapClient>(sessionId, out var cachedClient) &&
            cachedClient is IDisposable disposable)
        {
            disposable.Dispose();
        }

        cache.Remove(sessionId);
    }
}