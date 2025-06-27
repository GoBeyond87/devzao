using Microsoft.AspNetCore.Mvc;
using WebApi.Messaging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

/// <summary>
/// Controlador para testes de integração com Kafka
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class KafkaTestController : ControllerBase
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<KafkaTestController> _logger;
    private const string Topic = "meu-topico-teste";

    public KafkaTestController(IKafkaProducer kafkaProducer, ILogger<KafkaTestController> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    /// <summary>
    /// Envia uma mensagem para o tópico do Kafka
    /// </summary>
    /// <param name="request">Objeto contendo a mensagem a ser enviada</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("enviar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    [SwaggerOperation(
        Summary = "Envia uma mensagem para o tópico do Kafka",
        Description = "Envia a mensagem fornecida para o tópico 'meu-topico-teste' no Kafka"
    )]
    public async Task<IActionResult> EnviarMensagem([FromBody] EnviarMensagemRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Mensagem))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Mensagem inválida",
                Detail = "A mensagem não pode ser vazia",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var sucesso = await _kafkaProducer.ProduceAsync(Topic, request.Mensagem);
            
            if (sucesso)
            {
                _logger.LogInformation($"Mensagem enviada com sucesso: {request.Mensagem}");
                return Ok(new ApiResponse
                {
                    Sucesso = true,
                    Mensagem = "Mensagem enviada com sucesso",
                    Dados = new { mensagem = request.Mensagem, topico = Topic }
                });
            }
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro ao enviar mensagem",
                Detail = "Não foi possível enviar a mensagem para o Kafka",
                Status = StatusCodes.Status500InternalServerError
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar mensagem para o Kafka: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro interno",
                Detail = $"Ocorreu um erro ao processar sua solicitação: {ex.Message}",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}

/// <summary>
/// Modelo de requisição para envio de mensagem
/// </summary>
public class EnviarMensagemRequest
{
    /// <summary>
    /// Mensagem a ser enviada para o Kafka
    /// </summary>
    /// <example>Olá, esta é uma mensagem de teste</example>
    [Required(ErrorMessage = "O campo mensagem é obrigatório")]
    [StringLength(1000, ErrorMessage = "A mensagem não pode ter mais que 1000 caracteres")]
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// Modelo padrão de resposta da API
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    /// <example>true</example>
    public bool Sucesso { get; set; }
    
    /// <summary>
    /// Mensagem descritiva sobre o resultado da operação
    /// </summary>
    /// <example>Operação realizada com sucesso</example>
    public string Mensagem { get; set; } = string.Empty;
    
    /// <summary>
    /// Dados adicionais retornados pela operação
    /// </summary>
    public object? Dados { get; set; }
}
