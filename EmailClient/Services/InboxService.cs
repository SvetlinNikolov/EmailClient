namespace EmailClient.Services;

using EmailClient.Auth;
using EmailClient.Domain.Cache;
using EmailClient.Domain.Results;
using EmailClient.Factories.Contracts;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email;

public class InboxService(
    IImapClientFactory clientFactory,
    ICacheService cache,
    ICookieAuthService cookieAuthService) : IInboxService
{
    public async Task<Result> GetInboxAsync(string sessionId, int page, int pageSize)
    {
        var cacheKey = CacheConfig.GetInboxCacheKey(sessionId);
        if (cache.TryGet(cacheKey, out InboxViewModel cached) && cached.CurrentPage == page)
        {
            return Result.Success(cached);
        }

        var loginCookieResult = cookieAuthService.GetLoginCookie();
        if (!loginCookieResult.IsSuccess) return loginCookieResult;

        var login = loginCookieResult.GetData<LoginCookie>();
        var client = clientFactory.Create(login.ImapServer,
                                          login.ImapPort,
                                          login.ImapUsername,
                                          login.ImapPassword);

        var connect = await client.ConnectAsync();
        if (!connect.IsSuccess) return connect;

        var loginResult = await client.LoginAsync();
        if (!loginResult.IsSuccess) return loginResult;

        var headersResult = await client.GetInboxAsync(page, pageSize);
        if (!headersResult.IsSuccess) return headersResult;

        var inboxVm = headersResult.GetData<InboxViewModel>();

        cache.Set(cacheKey, inboxVm, CacheConfig.InboxTtl);

        return Result.Success(inboxVm);
    }
}
