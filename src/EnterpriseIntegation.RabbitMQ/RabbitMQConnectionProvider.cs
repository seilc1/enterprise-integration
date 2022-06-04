using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using static EnterpriseIntegation.RabbitMQ.IRabbitMQConnectionProvider;

namespace EnterpriseIntegation.RabbitMQ
{
    public class RabbitMQConnectionProvider : IRabbitMQConnectionProvider, IDisposable
    {
        private readonly IOptions<RabbitMQSettings> _settings;

        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQConnectionProvider(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _settings.Value.Hostname,
                UserName = _settings.Value.Username,
                Password = _settings.Value.Password,
                DispatchConsumersAsync = true
            };
        }

        private IConnection _connection;

        public IConnection Connection
        {
            get { return _connection ?? (Connection = _connectionFactory.CreateConnection()); }
            private set { 
                _connection = value;
                ConnectionChanged?.Invoke(this, _connection);
            }
        }

        /// <summary>
        ///     Sends <see cref="ConnectionChangedEvent"/> when the Connection has been updated (reconnect).
        /// </summary>
        public event ConnectionChangedEvent ConnectionChanged;

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}