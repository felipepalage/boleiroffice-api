using System.Text.Json;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Boleiroffice.Infrastructure.Caching;

public sealed class DistributedCacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
        return cachedValue is null ? default : JsonSerializer.Deserialize<T>(cachedValue, SerializerOptions);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        return _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
    {
        var serialized = JsonSerializer.Serialize(value, SerializerOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        return _distributedCache.SetStringAsync(key, serialized, options, cancellationToken);
    }
}
