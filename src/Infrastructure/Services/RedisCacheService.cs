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

        public async Task<T?> GetAsync<T>(string key) where T : class
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

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = expiry.HasValue 
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry }
                : _defaultOptions;

            var json = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return value != null;
        }
    }
}
