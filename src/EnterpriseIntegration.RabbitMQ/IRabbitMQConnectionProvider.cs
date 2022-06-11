using RabbitMQ.Client;

namespace EnterpriseIntegation.RabbitMQ;
public interface IRabbitMQConnectionProvider : IDisposable
{
    public delegate void ConnectionChangedEvent(RabbitMQConnectionProvider provider, IConnection connection);

    public IConnection Connection { get; }

    /// <summary>
    ///     Sends <see cref="ConnectionChangedEvent"/> when the Connection has been updated (reconnect).
    /// </summary>
    public event ConnectionChangedEvent ConnectionChanged;
}