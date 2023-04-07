using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Message;
using Microsoft.Extensions.Logging;

namespace EnterpriseIntegration.Kafka;

public class KafkaChannel : IMessagingChannel, IDisposable
{
    /// <summary>
    ///     Kafka is a highly distributed system, creating a subscription takes time to be synchronized across all nodes.
    /// </summary>
    private const int KafkaSubscriptionDelayInMs = 3_000;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly Lazy<IProducer<string, byte[]>> _producer;

    private readonly Lazy<IConsumer<string, byte[]>> _consumer;

    private readonly KafkaChannelSettings _settings;

    private readonly IMessageMapper<ConsumeResult<string, byte[]>, KafkaMessage> _mapper;

    private readonly ILogger<KafkaChannel> _logger;

    private readonly Task? _topicCreationTask;

    private bool _topicCreated = true;

    public ChannelId ChannelId { get; }

    public KafkaChannel(ChannelId id, IKafkaConnectionProvider connectionProvider, KafkaChannelSettings settings, IMessageMapper<ConsumeResult<string, byte[]>, KafkaMessage> mapper, ILogger<KafkaChannel> logger)
    {
        ChannelId = id;
        _settings = settings;
        _mapper = mapper;
        _logger = logger;
        _producer = new Lazy<IProducer<string, byte[]>>(connectionProvider.CreateProducer);
        _consumer = new Lazy<IConsumer<string, byte[]>>(connectionProvider.CreateConsumer);

        if (settings.EnsureCreated)
        {
            _topicCreated = false;
            _topicCreationTask = EnsureTopicCreationAsync(connectionProvider);
        }
    }

    private async Task EnsureTopicCreationAsync(IKafkaConnectionProvider connectionProvider)
    {
        using var adminClient = connectionProvider.CreateAdminClient();

        var metaData = adminClient.GetMetadata(TimeSpan.FromSeconds(30));
        if (!metaData.Topics.Any(t => t.Topic.Equals(_settings.TopicName, StringComparison.OrdinalIgnoreCase)))
        {
            await adminClient.CreateTopicsAsync(new List<TopicSpecification> { new TopicSpecification { Name = _settings.TopicName, NumPartitions = 1, ReplicationFactor = 1, } });
            _topicCreated = true;
        }
    }

    public async Task Send(IMessage message)
    {
        if (!_topicCreated && _topicCreationTask != null)
        {
            await _topicCreationTask;
        }

        _logger.LogInformation("Produce Message(id:{id}) for Channel:{channelId} to topic:{topic}", message.MessageHeaders.Id, ChannelId, _settings.TopicName);
        await _producer.Value.ProduceAsync(_settings.TopicName, await _mapper.Map(message));
    }

    /// <summary>
    ///     Reads messages from a kafka topic.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subscriber"></param>
    /// <returns></returns>
    public async Task Subscribe<T>(Func<IMessage<T>, Task> subscriber)
    {
        if (!_topicCreated && _topicCreationTask != null)
        {
            await _topicCreationTask;
        }

        _logger.LogInformation("Created subscription for channel:{channelId} on topic:{topic}", ChannelId, _settings.TopicName);
        _consumer.Value.Subscribe(_settings.TopicName);
        ThreadPool.QueueUserWorkItem(async _ => await ContinuousConsume(subscriber));
        await Task.Delay(KafkaSubscriptionDelayInMs);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_consumer.IsValueCreated)
        {
            _consumer.Value.Unsubscribe();
            _consumer.Value.Close();
            _consumer.Value.Dispose();
        }

        if (_producer.IsValueCreated)
        {
            _producer.Value.Dispose();
        }

        _cancellationTokenSource.Cancel();
    }

    private async Task ContinuousConsume<T>(Func<IMessage<T>, Task> subscriber)
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Consumer starting.");
                IMessage<T> message = await _mapper.Map<T>(_consumer.Value.Consume(_cancellationTokenSource.Token));
                _logger.LogInformation("Received Message(id:{id}) for Channel:{channelId} from topic:{topic}", message.MessageHeaders.Id, ChannelId, _settings.TopicName);

                await subscriber(message);
                _logger.LogInformation("Processed Message(id:{id})", message.MessageHeaders.Id);
            }
            catch (ObjectDisposedException)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                throw;
            }
            catch (OperationCanceledException)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                throw;
            }
        }
    }
}
