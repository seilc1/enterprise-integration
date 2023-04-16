using Xunit;

namespace EnterpriseIntegration.Kafka.Tests;

[CollectionDefinition("Kafka")]
public class KafkaFixtureCollection : ICollectionFixture<KafkaFixture> { }