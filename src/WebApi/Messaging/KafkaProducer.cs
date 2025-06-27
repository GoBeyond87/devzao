using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace WebApi.Messaging;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private const string BootstrapServers = "localhost:9092";

    public KafkaProducer(ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = BootstrapServers,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<Null, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError($"Erro no produtor: {e.Reason}"))
            .SetLogHandler((_, logMessage) => 
                _logger.LogInformation(
                    "Facility: {Facility} Level: {Level} Message: {Message}", 
                    logMessage.Facility, 
                    logMessage.Level, 
                    logMessage.Message))
            .Build();
    }

    public async Task<bool> ProduceAsync(string topic, string message)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            _logger.LogInformation($"Mensagem enviada para {result.TopicPartitionOffset}");
            return true;
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError($"Falha ao enviar mensagem: {ex.Error.Reason}");
            return false;
        }
    }

    public void Dispose()
    {
        try
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao liberar recursos do produtor");
        }
        GC.SuppressFinalize(this);
    }
}