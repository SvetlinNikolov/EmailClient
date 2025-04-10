namespace EmailClient.Services;

using EmailClient.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Set<T>(string key, T value, TimeSpan? ttl = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (ttl.HasValue)
        {
            options.SetAbsoluteExpiration(ttl.Value);
        }

        _cache.Set(key, value, options);
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_cache.TryGetValue(key, out var cached) && cached is T casted)
        {
            value = casted;
            return true;
        }

        value = default!;
        return false;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}