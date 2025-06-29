using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _defaultOptions;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
        }

        async Task<T?> ICacheService.GetAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var value = await _cache.GetStringAsync(key);
            if (value == null) 
                return default;
            
            try
            {
                return JsonSerializer.Deserialize<T>(value) ?? default;
            }
            catch (JsonException)
            {
                // Em caso de erro na desserialização, remove o valor inválido do cache
                await _cache.RemoveAsync(key);
                return default;
            }
        }

        async Task ICacheService.SetAsync<T>(string key, T value, TimeSpan? expiry) where T : class
        {
            var options = expiry.HasValue 
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry }
                : _defaultOptions;

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        Task ICacheService.RemoveAsync(string key)
        {
            return _cache.RemoveAsync(key);
        }

        async Task<bool> ICacheService.ExistsAsync(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return value != null;
        }
    }
}
