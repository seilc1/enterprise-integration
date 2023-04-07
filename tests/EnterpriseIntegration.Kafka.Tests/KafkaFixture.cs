using Confluent.Kafka;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace EnterpriseIntegration.Kafka.Tests;

public sealed class KafkaFixture : IDisposable
{
    public const string ZooKeeperPort = "2181";

    public const int KafkaPort = 29092;

    private ICompositeService Container { get; set; }

    public KafkaFixture()
    {
        // see: https://docs.confluent.io/platform/current/installation/docker/config-reference.html#confluent-ak-configuration
        var file = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.yml");

        Container = new Ductus.FluentDocker.Builders.Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(file)
            .WaitForPort("kafka", "29092/tcp")
            .Wait("kafka", WaitForServerStartedLogMessage)
            .Build()
            .Start();
    }

    public KafkaSettings Settings => new KafkaSettings
    {
        ConsumerConfig = new ConsumerConfig
        {
            GroupId = "test1",
            BootstrapServers = "127.0.0.1:29092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        },
        ProducerConfig = new ProducerConfig
        {
            BootstrapServers = "127.0.0.1:29092"
        }
    };

    private int WaitForServerStartedLogMessage(IContainerService service, int repeat)
        => service.Logs().ReadToEnd().Any(it => it.Contains("started (kafka.server.KafkaServer)")) || repeat >= 100 ? 0 : 100;

    public void Dispose()
    {
        Container.Dispose();
    }
}
