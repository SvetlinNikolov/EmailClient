using EmailClient.Domain.Results;
namespace EmailClient.Services.Contracts;

public interface IInboxService
{
    Task<Result> GetInboxAsync(string sessionId, int page, int pageSize, bool refresh);
}
