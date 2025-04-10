namespace EmailClient.Domain.Cache;

public static class CacheConfig
{
    public static string GetLoginCacheKey(string sessionId) => $"login:{sessionId}";

    public static readonly TimeSpan LoginCredentialsTtl = TimeSpan.FromMinutes(30);

    public static string GetInboxCacheKey(string sessionId) => $"inbox:{sessionId}";

    public static readonly TimeSpan InboxTtl = TimeSpan.FromSeconds(60);
}
