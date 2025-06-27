using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Interfaces;
using Infrastructure.Services;  
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class RedisCacheServiceTests
    {
        private readonly RedisCacheService _cacheService;
        private readonly IDistributedCache _distributedCache;
        private readonly Mock<IDistributedCache> _mockDistributedCache;
        private readonly string _testKey = "test_key";
        private readonly string _testValue = "test_value";

        public RedisCacheServiceTests()
        {
            // Configuração do cache em memória para testes
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            _distributedCache = new MemoryDistributedCache(opts);
            
            _mockDistributedCache = new Mock<IDistributedCache>();
            _cacheService = new RedisCacheService(_distributedCache);
        }

        [Fact]
        public async Task SetAsync_WhenCalled_StoresValueInCache()
        {
            // Act
            await _cacheService.SetAsync(_testKey, _testValue);
            
            // Assert
            var storedValue = await _distributedCache.GetStringAsync(_testKey);
            Assert.NotNull(storedValue);
            Assert.Equal(JsonSerializer.Serialize(_testValue), storedValue);
        }

        [Fact]
        public async Task GetAsync_WhenValueExists_ReturnsValue()
        {
            // Arrange
            await _cacheService.SetAsync(_testKey, _testValue);
            
            // Act
            var result = await _cacheService.GetAsync<string>(_testKey);
            
            // Assert
            Assert.Equal(_testValue, result);
        }

        [Fact]
        public async Task GetAsync_WhenValueNotExists_ReturnsDefault()
        {
            // Act
            var result = await _cacheService.GetAsync<string>("non_existing_key");
            
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_WhenCalled_RemovesValueFromCache()
        {
            // Arrange
            await _cacheService.SetAsync(_testKey, _testValue);
            
            // Act
            await _cacheService.RemoveAsync(_testKey);
            
            // Assert
            var result = await _distributedCache.GetStringAsync(_testKey);
            Assert.Null(result);
        }

        [Fact]
        public async Task ExistsAsync_WhenKeyExists_ReturnsTrue()
        {
            // Arrange
            await _cacheService.SetAsync(_testKey, _testValue);
            
            // Act
            var exists = await _cacheService.ExistsAsync(_testKey);
            
            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_WhenKeyNotExists_ReturnsFalse()
        {
            // Act
            var exists = await _cacheService.ExistsAsync("non_existing_key");
            
            // Assert
            Assert.False(exists);
        }
    }
}
