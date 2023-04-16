using Confluent.Kafka;
namespace EnterpriseIntegration.Kafka;

public interface IKafkaConnectionProvider
{
    public IProducer<string, byte[]> CreateProducer();

    public IConsumer<string, byte[]> CreateConsumer();

    public IAdminClient CreateAdminClient();
}