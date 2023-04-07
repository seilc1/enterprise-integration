using Confluent.Kafka;

namespace EnterpriseIntegration.Kafka;

public class KafkaSettings
{
    /// <summary>
    ///     Config Path in the appsettings, where this configuration is defined.
    /// </summary>
    public const string ConfigPath = "EnterpriseIntegration:Kafka";

    public ConsumerConfig ConsumerConfig { get; set; } = new ConsumerConfig();

    public ProducerConfig ProducerConfig { get; set; } = new ProducerConfig();
}
