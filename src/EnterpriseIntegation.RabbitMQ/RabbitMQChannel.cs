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
        private readonly IConnection _connection;

        private readonly string _queueName;

        private readonly RabbitMQChannelSettings _settings;

        private readonly IMessageTransformer _transformer;

        private readonly Lazy<IModel> _queue;

        private readonly Encoding _headerEncoding = Encoding.UTF8;

        public bool HasSubscriber { get; private set; }

        public RabbitMQChannel(IConnection connection, string queueName, IMessageTransformer transformer) : this(connection, queueName, new RabbitMQChannelSettings(), transformer) { }

        public RabbitMQChannel(IConnection connection, string queueName, RabbitMQChannelSettings settings, IMessageTransformer transformer)
        {
            _connection = connection;
            _queueName = queueName;
            _settings = settings;
            _transformer = transformer;
            _queue = new Lazy<IModel>(CreateQueue);
        }

        private IModel CreateQueue()
        {
            IModel channel = _connection.CreateModel();
            channel.QueueDeclare(queue: _queueName,
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
                    routingKey: _queueName,
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

            return Task.Run(() =>
            {
                var queue = _queue.Value;
                var consumer = new EventingBasicConsumer(_queue.Value);
                consumer.Received += async (_, ea) =>
                {
                    T payload = await _transformer.Deserialize<T>(ea.Body);
                    await subscriber(new GenericMessage<T>(GetHeaders(ea.BasicProperties), payload));
                };

                queue.BasicConsume(_queueName, _settings.AutoAcknowledge, consumer);
                HasSubscriber = true;
            });
        }

        private Task HandleMessage<T>(BasicDeliverEventArgs data, Func<IMessage<T>, Task> subscriber)
        {
            return Task.CompletedTask;
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