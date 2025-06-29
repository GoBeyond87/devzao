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
    class TestObject
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class RedisCacheServiceTests
    {
        private readonly ICacheService _cacheService;
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
            // Arrange
            var value = new TestObject { Id = 1, Name = "Test" };
            
            // Act
            await _cacheService.SetAsync(_testKey, value, default);
            
            // Assert
            var storedValue = await _cacheService.GetAsync<TestObject>(_testKey);
            Assert.NotNull(storedValue);
            Assert.Equal(value.Id, storedValue.Id);
            Assert.Equal(value.Name, storedValue.Name);
        }

        [Fact]
        public async Task GetAsync_WhenValueExists_ReturnsValue()
        {
            // Arrange
            var value = new TestObject { Id = 1, Name = "Test" };
            await _cacheService.SetAsync(_testKey, value, default);
            
            // Act
            var result = await _cacheService.GetAsync<TestObject>(_testKey);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Id, result.Id);
            Assert.Equal(value.Name, result.Name);
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
            await _cacheService.SetAsync(_testKey, _testValue, default);
            
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
            await _cacheService.SetAsync(_testKey, _testValue, default);
            
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
