using Xunit;

namespace EnterpriseIntegration.RabbitMQ.Tests;

[CollectionDefinition("RabbitMQ")]
public class RabbitMQCollection : ICollectionFixture<RabbitMQFixture> { }