using EnterpriseIntegration.Channels;
using EnterpriseIntegration.Errors;
using EnterpriseIntegration.Message;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EnterpriseIntegation.RabbitMQ
{
    public class RabbitMQChannel : IMessagingChannel, IDisposable
    {
        private readonly IRabbitMQConnectionProvider _connectionProvider;

        private readonly RabbitMQChannelSettings _settings;

        private readonly IMessageTransformer _transformer;

        private readonly Lazy<IModel> _queue;

        private readonly Encoding _headerEncoding = Encoding.UTF8;

        public bool HasSubscriber { get; private set; }

        public ChannelId ChannelId { get; init; }

        public RabbitMQChannel(ChannelId id, IRabbitMQConnectionProvider connectionProvider, IMessageTransformer transformer) 
            : this(id, connectionProvider, new RabbitMQChannelSettings(id.ToString()), transformer) { }

        public RabbitMQChannel(ChannelId id, IRabbitMQConnectionProvider connectionProvider, RabbitMQChannelSettings settings, IMessageTransformer transformer)
        {
            ChannelId = id;
            _connectionProvider = connectionProvider;
            _settings = settings;
            _transformer = transformer;
            _queue = new Lazy<IModel>(CreateQueue);
        }

        private IModel CreateQueue()
        {
            IModel channel = _connectionProvider.Connection.CreateModel();
            channel.QueueDeclare(queue: _settings.QueueName,
                                     durable: _settings.Durable,
                                     exclusive: _settings.Exclusive,
                                     autoDelete: _settings.AutoDelete,
                                     arguments: _settings.Arguments);
            return channel;
        }

        public async Task Send(IMessage message)
        {
            ReadOnlyMemory<byte> payload = await _transformer.Serialize(message.Payload);
            await Task.Run(() =>
            {
                var channel = _queue.Value;
                channel.BasicPublish(
                    exchange: _settings.Exchange,
                    routingKey: _settings.QueueName,
                    mandatory: _settings.Mandatory,
                    basicProperties: AsBasicProperties(channel, message.MessageHeaders, message.PayloadType),
                    body: payload);
            });
        }

        private IBasicProperties AsBasicProperties(IModel queue, IMessageHeaders headers, Type payloadType)
        {
            var properties = queue.CreateBasicProperties();

            properties.MessageId = headers.Id;
            properties.Type = payloadType.FullName;
            properties.Headers = headers.ToDictionary(h => h.Key, h => (object)h.Value);
            properties.ContentType = _transformer.ContentType;
            properties.ContentEncoding = _transformer.Encoding;

            return properties;
        }

        public Task Subscribe<T>(Func<IMessage<T>, Task> subscriber)
        {
            if (HasSubscriber)
            {
                throw new EnterpriseIntegrationException($"{nameof(PointToPointDirectMessagingChannel)} only supports a single subscriber.");
            }
            HasSubscriber = true;

            return Task.Run(() => _queue.Value.BasicConsume(_settings.QueueName, _settings.AutoAcknowledge, Consumer(subscriber)));
        }

        private AsyncDefaultBasicConsumer Consumer<T>(Func<IMessage<T>, Task> subscriber)
        {
            var consumer = new AsyncEventingBasicConsumer(_queue.Value);
            consumer.Received += async (_, ea) =>
            {
                await subscriber(new GenericMessage<T>(GetHeaders(ea.BasicProperties), await _transformer.Deserialize<T>(ea.Body)));
            };

            return consumer;
        }

        private MessageHeaders GetHeaders(IBasicProperties messageProperties)
        {
            return new MessageHeaders(messageProperties.Headers.ToDictionary(h => h.Key, h => _headerEncoding.GetString((byte[])h.Value)));
        }

        public void Dispose()
        {
            if (_queue.IsValueCreated)
            {
                _queue.Value.Dispose();
            }
        }
    }
}