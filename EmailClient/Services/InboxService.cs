namespace EmailClient.Services;

using EmailClient.Auth;
using EmailClient.Domain.Results;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email;
using EmailClient.ViewModels.Login;

public class InboxService(IIMapSessionService imapSessionService, ICookieAuthService cookieAuthService) : IInboxService
{
    public async Task<Result> GetInboxAsync(string sessionId, int page, int perPage, bool refresh)
    {
        var cookieResult = cookieAuthService.GetLoginCookie();
        if (!cookieResult.IsSuccess)
        {
            return cookieResult;
        }

        var clientResult = await imapSessionService.GetOrCreateAsync(sessionId, cookieResult.GetData<LoginCookie>());
        if (!clientResult.IsSuccess)
        {
            return clientResult;
        }

        var client = clientResult.GetData<IImapClient>();

        var fetchResult = await client.GetInboxAsync(page, perPage);
        if (!fetchResult.IsSuccess)
        {
            return fetchResult;
        }

        var inboxVm = fetchResult.GetData<InboxViewModel>();

        return Result.Success(new InboxViewModel
        {
            Emails = inboxVm.Emails,
            CurrentPage = page,
            TotalPages = inboxVm.TotalPages
        });
    }

}
