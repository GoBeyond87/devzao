using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Tests.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Application.Tests.Controllers
{
    public class CacheControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ICacheService _cacheService;
        private const string BaseUrl = "/api/cache";
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed;

        public CacheControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

            _scope = _factory.Services.CreateScope();
            _cacheService = _scope.ServiceProvider.GetRequiredService<ICacheService>();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        [Fact]
        public async Task Get_WhenKeyDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistentKey = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{nonExistentKey}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task SetAndGet_WhenValidData_ReturnsCachedValue()
        {
            // Arrange
            var key = "test_key";
            var value = "test_value";
            var request = new { Key = key, Value = value };
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions), 
                Encoding.UTF8, 
                "application/json");

            // Act - Set
            var setResponse = await _client.PostAsync(BaseUrl, content);
            setResponse.EnsureSuccessStatusCode();

            // Act - Get
            var getResponse = await _client.GetAsync($"{BaseUrl}/{key}");
            getResponse.EnsureSuccessStatusCode();

            var responseContent = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CacheResponse>(responseContent, _jsonOptions);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(key, result.Key);
            Assert.Equal(value, result.Value);
        }

        [Fact]
        public async Task Delete_WhenKeyExists_RemovesFromCache()
        {
            // Arrange
            var key = "key_to_delete";
            var value = "value_to_delete";
            
            // First, add the key
            var request = new { Key = key, Value = value };
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions), 
                Encoding.UTF8, 
                "application/json");
                
            var setResponse = await _client.PostAsync(BaseUrl, content);
            setResponse.EnsureSuccessStatusCode();

            // Act - Delete
            var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{key}");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await _client.GetAsync($"{BaseUrl}/{key}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Set_WithExpiration_ExpiresAfterTime()
        {
            // Arrange
            var key = "expiring_key";
            var value = "expiring_value";
            var expirationSeconds = 2;

            // Create content with expiration
            var request = new { Key = key, Value = value, ExpirationInSeconds = expirationSeconds };
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions), 
                Encoding.UTF8, 
                "application/json");

            // Act - Set with expiration
            var setResponse = await _client.PostAsync(BaseUrl, content);
            setResponse.EnsureSuccessStatusCode();

            // Act - Get before expiration (should exist)
            var getResponse1 = await _client.GetAsync($"{BaseUrl}/{key}");
            getResponse1.EnsureSuccessStatusCode();

            // Wait for expiration
            await Task.Delay(TimeSpan.FromSeconds(expirationSeconds + 1));

            // Act - Get after expiration (should not exist)
            var getResponse2 = await _client.GetAsync($"{BaseUrl}/{key}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getResponse2.StatusCode);
        }


        [Fact]
        public void CacheService_IsRegistered_ReturnsInstance()
        {
            // Act
            var cacheService = _scope.ServiceProvider.GetRequiredService<ICacheService>();

            // Assert
            Assert.NotNull(cacheService);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client?.Dispose();
                    _scope?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class CacheResponse
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}
