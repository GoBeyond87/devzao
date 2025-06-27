using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Admin; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApi.Messaging;

public class KafkaConsumerService : BackgroundService
{
    private IConsumer<Ignore, string>? _consumer; 
    private readonly ILogger<KafkaConsumerService> _logger;
    private const string Topic = "meu-topico-teste";
    private const string BootstrapServers = "localhost:9092";
    private readonly ConsumerConfig _consumerConfig;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = "meu-grupo-consumidor",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando serviço de consumo do Kafka...");
        
        try
        {
            // Aguarda 10 segundos para garantir que o Kafka esteja pronto
            _logger.LogInformation("Aguardando 10 segundos para garantir que o Kafka esteja pronto...");
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            // Aguarda o Kafka ficar disponível
            await WaitForKafka(stoppingToken);

            // Cria o tópico se não existir
            await CreateTopicIfNotExistsAsync();

            _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig)
                .SetErrorHandler((_, e) => _logger.LogError($"Erro no consumidor: {e.Reason}"))
                .Build();

            _consumer.Subscribe(Topic);
            _logger.LogInformation($"Consumidor configurado e inscrito no tópico: {Topic}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult != null)
                    {
                        _logger.LogInformation($"Mensagem recebida: {consumeResult.Message.Value}");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Erro ao consumir mensagem: {ex.Error.Reason}");
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operação de consumo cancelada");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro inesperado: {ex.Message}");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro fatal no consumidor Kafka: {ex.Message}");
            throw;
        }
    }

    private async Task CreateTopicIfNotExistsAsync()
    {
        try
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig 
            { 
                BootstrapServers = BootstrapServers 
            }).Build();

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var topicExists = metadata.Topics.Any(t => t.Topic == Topic);

            if (!topicExists)
            {
                _logger.LogInformation($"Criando tópico: {Topic}");
                await adminClient.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                    {
                        Name = Topic,
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    }
                });
                _logger.LogInformation($"Tópico {Topic} criado com sucesso");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar/criar tópico");
            throw;
        }
    }

    private async Task WaitForKafka(CancellationToken stoppingToken)
    {
        const int maxAttempts = 10;
        int attempt = 0;

        while (!stoppingToken.IsCancellationRequested && attempt < maxAttempts)
        {
            try
            {
                using var adminClient = new AdminClientBuilder(new AdminClientConfig 
                { 
                    BootstrapServers = BootstrapServers,
                    SocketTimeoutMs = 5000
                }).Build();

                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                _logger.LogInformation("Conexão com Kafka estabelecida com sucesso!");
                return;
            }
            catch (Exception ex)
            {
                attempt++;
                _logger.LogWarning($"Tentativa {attempt}/{maxAttempts} - Kafka não disponível: {ex.Message}");
                if (attempt >= maxAttempts)
                {
                    _logger.LogError("Número máximo de tentativas atingido. Verifique se o Kafka está rodando corretamente.");
                    throw new InvalidOperationException("Não foi possível conectar ao Kafka após várias tentativas", ex);
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        try
        {
            _consumer?.Close();
            _consumer?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao liberar recursos do consumidor");
        }
        base.Dispose();
    }
}