using Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public MemoryCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        async Task<T?> ICacheService.GetAsync<T>(string key) where T : class
        {
            var value = await _cache.GetStringAsync(key);
            return value != null ? JsonSerializer.Deserialize<T>(value) : default;
        }

        async Task ICacheService.SetAsync<T>(string key, T value, TimeSpan? expiry) where T : class
        {
            var options = expiry.HasValue 
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry }
                : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        async Task ICacheService.RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        async Task<bool> ICacheService.ExistsAsync(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return value != null;
        }
    }
}
