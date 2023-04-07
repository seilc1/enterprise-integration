using Microsoft.Extensions.Options;
using Confluent.Kafka;

namespace EnterpriseIntegration.Kafka;

public class KafkaConnectionProvider : IKafkaConnectionProvider
{
    private readonly KafkaSettings _settings;

    public KafkaConnectionProvider(IOptions<KafkaSettings> settings)
    {
        _settings = settings.Value;
    }

    public IConsumer<string, byte[]> CreateConsumer()
        => new ConsumerBuilder<string, byte[]>(_settings.ConsumerConfig).Build();

    public IProducer<string, byte[]> CreateProducer()
        => new ProducerBuilder<string, byte[]>(_settings.ProducerConfig).Build();

    public IAdminClient CreateAdminClient()
        => new AdminClientBuilder(_settings.ProducerConfig).Build();
}
