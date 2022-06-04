namespace EnterpriseIntegation.RabbitMQ;

public class RabbitMQChannelSettings
{
    public RabbitMQChannelSettings(string queueName)
    {
        QueueName = queueName;
    }

    /// <summary>
    ///     Name of the Queue defined in RabbitMQ.
    /// </summary>
    public string QueueName { get; set; }

    /// <summary>
    ///     Defines the queue as durable: it will survive a broker restart.
    /// </summary>
    /// <see href="https://www.rabbitmq.com/queues.html#properties"/>
    public bool Durable { get; set; } = false;

    /// <summary>
    ///     Defines the queue as to be exclusively used by the creator. 
    ///     Other parties connected to RabbitMQ will not see this queue.
    /// </summary>
    /// <see href="https://www.rabbitmq.com/queues.html#properties"/>
    /// <see href="https://www.rabbitmq.com/queues.html#exclusive-queues"/>
    public bool Exclusive { get; set; } = false;

    /// <summary>
    ///     Defines the queue to automatically delete itself, as soon as
    ///     the last subscriber disconnected. at least one subscriber must
    ///     have been connected.
    /// </summary>
    /// <see href="https://www.rabbitmq.com/queues.html#properties"/>
    public bool AutoDelete { get; set; } = true;

    /// <summary>
    ///     Allows passing additional arguments to the queue (creation).
    ///     Those informations can be used by plugins
    /// </summary>
    /// <see href="https://www.rabbitmq.com/queues.html#properties"/>
    public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

    /// <summary>
    ///     Defines handling if message cannot be successfully published.
    /// </summary>
    /// <remarks>
    ///     Default: true. RabbitMQ will send back the message/error to be handled by the library.
    /// </remarks>
    /// <see href="https://www.rabbitmq.com/publishers.html#protocols"/>
    public bool Mandatory { get; set; } = true;

    /// <summary>
    ///     Defines the Exchange type to be used when publishing Messages to this queue.
    /// </summary>
    /// <remarks>
    ///     The default is <see cref="string.Empty"/> which is the built in direct channel.
    /// </remarks>
    /// <see href="https://www.rabbitmq.com/tutorials/amqp-concepts.html#exchanges"/>
    public string Exchange { get; set; } = string.Empty;

    /// <summary>
    ///     Defines if all messages should be automatically be acknowledged, when consumed.
    /// </summary>
    /// <see href="https://www.rabbitmq.com/confirms.html#acknowledgement-modes"/>
    public bool AutoAcknowledge { get; set; } = false;
}