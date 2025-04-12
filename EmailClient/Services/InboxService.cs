namespace EmailClient.Services;

using EmailClient.Auth;
using EmailClient.Domain.Cache;
using EmailClient.Domain.Results;
using EmailClient.Factories.Contracts;
using EmailClient.Helpers;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email;

public class InboxService(
    IImapClientFactory clientFactory,
    ICacheService cache,
    ICookieAuthService cookieAuthService,
    IPaginationService paginationService) : IInboxService
{
    public async Task<Result> GetInboxAsync(string sessionId, int page, int perPage, bool refresh)
    {
        //var key = CacheConfig.GetInboxCacheKey(sessionId);

        var loginResult = cookieAuthService.GetLoginCookie();
        if (!loginResult.IsSuccess) return Result.Failure(loginResult.Error!);

        var login = loginResult.GetData<LoginCookie>();
        var client = clientFactory.Create(login.ImapServer, login.ImapPort, login.ImapUsername, login.ImapPassword);

        var connected = await client.ConnectAndLoginAsync();
        if (!connected.IsSuccess) return Result.Failure(connected.Error!);

        var fetchResult = await client.GetInboxAsync(page, perPage);
        if (!fetchResult.IsSuccess) return Result.Failure(fetchResult.Error!);

        var inboxVm = fetchResult.GetData<InboxViewModel>();

        //cache.Set(key, inboxVm.Emails, CacheConfig.InboxTtl);

        //var paged = paginationService.Paginate(inboxVm.Emails!, page, perPage);
        return Result.Success(new InboxViewModel
        {
            Emails = paged,
            CurrentPage = page,
            TotalPages = inboxVm.TotalPages
        });
    }

}
