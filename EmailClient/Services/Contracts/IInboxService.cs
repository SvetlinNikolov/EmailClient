using EmailClient.Domain.Results;
using EmailClient.ViewModels.Login;

namespace EmailClient.Services.Contracts;

public interface IInboxService
{
    Task<Result> GetInboxAsync(string sessionId, int page, int pageSize);

}
