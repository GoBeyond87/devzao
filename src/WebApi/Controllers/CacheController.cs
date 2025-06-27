using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador para operações de cache
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheController> _logger;
        private readonly IDistributedCache _distributedCache;

        public CacheController(
            ICacheService cacheService, 
            ILogger<CacheController> logger,
            IDistributedCache distributedCache)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        /// <summary>
        /// Verifica se o serviço de cache está ativo
        /// </summary>
        /// <returns>Status do serviço de cache</returns>
        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> Ping()
        {
            try
            {
                await _distributedCache.GetStringAsync("ping_test");
                return Ok("PONG");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao acessar o Redis");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Serviço de cache indisponível");
            }
        }

        /// <summary>
        /// Obtém um valor do cache pela chave
        /// </summary>
        /// <param name="key">Chave do cache</param>
        /// <returns>Valor armazenado no cache</returns>
        [HttpGet("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CacheResponse>> Get(string key)
        {
            try
            {
                _logger.LogInformation("Getting value for key: {Key}", key);
                var value = await _cacheService.GetAsync<string>(key);
                
                if (value == null)
                {
                    _logger.LogInformation("Key not found: {Key}", key);
                    return NotFound();
                }

                return Ok(new CacheResponse { Key = key, Value = value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value for key: {Key}", key);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Verifica se uma chave existe no cache
        /// </summary>
        /// <param name="key">Chave a ser verificada</param>
        /// <returns>200 se a chave existir, 404 caso contrário</returns>
        [HttpHead("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Head(string key)
        {
            try
            {
                _logger.LogInformation("Checking if key exists: {Key}", key);
                var exists = await _cacheService.ExistsAsync(key);
                
                if (!exists)
                {
                    _logger.LogInformation("Key not found: {Key}", key);
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key exists: {Key}", key);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Define um valor no cache
        /// </summary>
        /// <param name="request">Dados para armazenar no cache</param>
        /// <returns>Status da operação</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Set([FromBody] CacheRequest request)
        {
            try
            {
                _logger.LogInformation("Setting value for key: {Key}", request.Key);
                
                if (request.ExpirationInSeconds.HasValue && request.ExpirationInSeconds > 0)
                {
                    await _cacheService.SetAsync(
                        request.Key, 
                        request.Value, 
                        TimeSpan.FromSeconds(request.ExpirationInSeconds.Value));
                }
                else
                {
                    await _cacheService.SetAsync(request.Key, request.Value);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value for key: {Key}", request?.Key);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Remove um valor do cache
        /// </summary>
        /// <param name="key">Chave a ser removida</param>
        /// <returns>Status da operação</returns>
        [HttpDelete("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string key)
        {
            try
            {
                _logger.LogInformation("Removing key: {Key}", key);
                await _cacheService.RemoveAsync(key);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing key: {Key}", key);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }

    /// <summary>
    /// Modelo de resposta para operações de cache
    /// </summary>
    public class CacheResponse
    {
        /// <summary>
        /// Chave do cache
        /// </summary>
        public required string Key { get; set; }
        
        /// <summary>
        /// Valor armazenado
        /// </summary>
        public required string Value { get; set; }
    }

    /// <summary>
    /// Modelo de requisição para operações de cache
    /// </summary>
    public class CacheRequest
    {
        /// <summary>
        /// Chave para armazenar no cache
        /// </summary>
        public required string Key { get; set; }
        
        /// <summary>
        /// Valor para armazenar
        /// </summary>
        public required string Value { get; set; }
        
        /// <summary>
        /// Tempo de expiração em segundos (opcional)
        /// </summary>
        public int? ExpirationInSeconds { get; set; }
    }
}
