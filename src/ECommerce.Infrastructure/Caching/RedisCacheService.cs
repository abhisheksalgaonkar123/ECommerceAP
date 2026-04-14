using System.Text.Json;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace ECommerce.Infrastructure.Caching;

public class RedisCacheService:ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
    {
        _cache = cache;
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var data = await _cache.GetStringAsync(key, ct);
        if (data is null) return default;
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
                expiry ?? TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(key, json, options, ct);

    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public async Task RemoveByPatternAsync(
        string pattern,
        CancellationToken ct)
    {
        var server = _connectionMultiplexer
            .GetServer(_connectionMultiplexer
                .GetEndPoints().First());

        // Add instance prefix to pattern!
        var fullPattern = $"ECommerce:{pattern}";

        var keys = server
            .Keys(pattern: fullPattern)
            .ToArray();

        foreach (var key in keys)
        {
            // Remove instance prefix before calling RemoveAsync
            // because IDistributedCache adds it automatically!
            var keyWithoutPrefix = key.ToString()
                .Replace("ECommerce:", "");

            await _cache.RemoveAsync(keyWithoutPrefix, ct);
        }
    }
}
