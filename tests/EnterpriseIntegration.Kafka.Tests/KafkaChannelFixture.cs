using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using EnterpriseIntegration.TestCommon;
using EnterpriseIntegration.Tests;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace EnterpriseIntegration.Kafka.Tests;

[Collection("Kafka")]
public class KafkaChannelFixture
{
    public static readonly TimeSpan CreateTopicDelay = TimeSpan.FromSeconds(2);

    private readonly KafkaFixture _kafkaFixture;

    private readonly KafkaConnectionProvider _connectionProvider;

    private readonly KafkaMessageMapper _messageMapper = new(new DefaultMessageTransformer());

    private readonly IAdminClient _adminClient;

    private readonly ILogger<KafkaChannel> _logger;

    public record Example(string Name, int Value);

    public KafkaChannelFixture(KafkaFixture kafkaFixture, ITestOutputHelper testOutputHelper)
    {
        _kafkaFixture = kafkaFixture;
        _connectionProvider = new KafkaConnectionProvider(new OptionsWrapper<KafkaSettings>(_kafkaFixture.Settings));
        _adminClient = new AdminClientBuilder(_kafkaFixture.Settings.ProducerConfig).Build();
        _logger = new XUnitLogger<KafkaChannel>(testOutputHelper);
    }

    [Fact]
    public async Task Channel_ShouldSendAndReceive()
    {
        // Arrange
        string topicName = $"T{Guid.NewGuid():N}";
        KafkaChannelSettings kafkaChannelSettings = new() { TopicName = topicName };
        await _adminClient.CreateTopicsAsync(new List<TopicSpecification>() { new TopicSpecification() { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 } });
        KafkaChannel channel = new(new ChannelId("TEST"), _connectionProvider, kafkaChannelSettings, _messageMapper, _logger);

        IMessage<Example> message = new GenericMessage<Example>(new Example("Test", 7));
        message.MessageHeaders.Add("custom_header", "some_value");

        IMessage<Example>? result = null;

        Task subscriber(IMessage<Example> msg) => Task.Run(() => result = msg);

        // Act
        await channel.Subscribe((Func<IMessage<Example>, Task>)subscriber);
        await channel.Send(message);
        await TestHelper.WaitFor(() => result != null, maxWaitTimeInMilliseconds: 10_000);

        // Assert
        result.Should().NotBeNull();
        result!.Payload.Should().NotBeNull();
        result.Payload.Name.Should().Be("Test");
        result.Payload.Value.Should().Be(7);

        result.MessageHeaders.Id.Should().Be(message.MessageHeaders.Id);
        result.MessageHeaders.CreatedDate.Should().Be(message.MessageHeaders.CreatedDate);
        result.MessageHeaders["custom_header"].Should().Be("some_value");
    }

    [Fact]
    public async Task Channel_ShouldSendAndReceive_Multiple()
    {
        // Arrange
        string topicName = $"T{Guid.NewGuid():N}";
        KafkaChannelSettings kafkaChannelSettings = new() { TopicName = topicName };
        await _adminClient.CreateTopicsAsync(new List<TopicSpecification>() { new TopicSpecification() { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 } });

        KafkaChannel channel = new(new ChannelId("TEST"), _connectionProvider, kafkaChannelSettings, _messageMapper, _logger);
        List<IMessage<Example>> messages = new(GenerateMessages(15));
        List<IMessage<Example>> results = new();
        Task subscriber(IMessage<Example> responseMessage) => Task.Run(() => results.Add(responseMessage));

        // Act
        await channel.Subscribe((Func<IMessage<Example>, Task>)subscriber);

        List<Task> sendingTasks = new(messages.Count);
        foreach (var message in messages)
        {
            sendingTasks.Add(channel.Send(message));
        }
        await Task.WhenAll(sendingTasks);
        await TestHelper.WaitFor(() => results.Count == messages.Count, maxWaitTimeInMilliseconds: 10_000);

        // Assert
        results.Should().NotBeNull();
        results.Count.Should().Be(messages.Count);

        foreach (IMessage<Example> message in messages)
        {
            IMessage<Example>? result = results.Find(m => m.MessageHeaders.Id == message.MessageHeaders.Id);

            result!.Payload.Should().NotBeNull();
            result.Payload.Name.Should().Be(message.Payload.Name);
            result.Payload.Value.Should().Be(message.Payload.Value);

            result.MessageHeaders.Id.Should().Be(message.MessageHeaders.Id);
            result.MessageHeaders.CreatedDate.Should().Be(message.MessageHeaders.CreatedDate);
        }
    }

    [Fact]
    public async Task Channel_ShouldSendAndReceive_SimpleType()
    {
        // Arrange
        string topicName = $"T{Guid.NewGuid():N}";
        KafkaChannelSettings kafkaChannelSettings = new() { TopicName = topicName };
        await _adminClient.CreateTopicsAsync(new List<TopicSpecification>() { new TopicSpecification() { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 } });

        KafkaChannel channel = new(new ChannelId("TEST"), _connectionProvider, kafkaChannelSettings, _messageMapper, _logger);
        IMessage<string> message = new GenericMessage<string>("test-payload");
        message.MessageHeaders.Add("custom_header", "some_value");

        IMessage<string>? result = null;
        Task subscriber(IMessage<string> msg) => Task.Run(() => result = msg);

        // Act
        await channel.Subscribe((Func<IMessage<string>, Task>)subscriber);
        await channel.Send(message);
        await TestHelper.WaitFor(() => result != null, maxWaitTimeInMilliseconds: 5_000);

        // Assert
        result.Should().NotBeNull();
        result!.Payload.Should().Be("test-payload");

        result.MessageHeaders.Id.Should().Be(message.MessageHeaders.Id);
        result.MessageHeaders.CreatedDate.Should().Be(message.MessageHeaders.CreatedDate);
    }

    private static IEnumerable<IMessage<Example>> GenerateMessages(int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new GenericMessage<Example>(new Example($"TEST_{i}", i));
        }
    }
}
