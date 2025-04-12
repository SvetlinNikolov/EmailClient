using EmailClient.Domain.Results;

namespace EmailClient.Services.Contracts;

public interface IImapClient : IDisposable
{
    Task<Result> ConnectAsync();
    Task<Result> LoginAsync();
    Task<Result> ConnectAndLoginAsync();
    Task<Result> LogoutAsync();
    Task<Result> GetInboxAsync(int page, int pageSize);
}
