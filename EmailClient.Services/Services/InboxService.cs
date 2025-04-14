namespace EmailClient.Services.Services;

using EmailClient.Imap.Client.Contracts;
using EmailClient.Imap.Models;
using EmailClient.Services.Auth;
using EmailClient.Services.Services.Contracts;
using EmailClient.ViewModels.ViewModels.Email;
using EmailService.Domain.Results;

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

        var inboxVm = fetchResult.GetData<InboxDto>();

        return Result.Success(new InboxViewModel
        {
            Emails = inboxVm.Emails!.Select(x => new EmailHeader(x.Subject, x.From, x.Date)),
            CurrentPage = page,
            TotalPages = inboxVm.TotalPages
        });
    }

}
