using EmailClient.Domain.Results;

namespace EmailClient.Services.Contracts;

public interface IImapClient : IDisposable
{
    Task<Result> ConnectAsync();
    Task<Result> LoginAsync();
    Task<Result> LogoutAsync();
    Task<Result> GetInboxAsync(int page = 1, int pageSize = 10);
}
