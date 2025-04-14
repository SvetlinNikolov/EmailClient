namespace EmailClient.Services.Services.Contracts;

public interface ICacheService
{
    void Set<T>(string key, T value, TimeSpan? ttl = null);
    bool TryGet<T>(string key, out T value);
    void Remove(string key);
}
