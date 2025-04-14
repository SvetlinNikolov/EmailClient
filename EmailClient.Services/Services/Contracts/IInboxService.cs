
using EmailService.Domain.Results;
namespace EmailClient.Services.Services.Contracts;

public interface IInboxService
{
    Task<Result> GetInboxAsync(string sessionId, int page, int pageSize, bool refresh);
}
