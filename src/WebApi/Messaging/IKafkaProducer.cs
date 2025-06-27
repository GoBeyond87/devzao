using Confluent.Kafka;

namespace WebApi.Messaging;

public interface IKafkaProducer
{
    Task<bool> ProduceAsync(string topic, string message);
}
