namespace EnterpriseIntegation.RabbitMQ;

public class RabbitMQChannelSettings
{
    public bool Durable { get; set; } = false;

    public bool Exclusive { get; set; } = false;

    public bool AutoDelete { get; set; } = false;

    public bool Mandatory { get; set; } = false;

    public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

    public string Exchange { get; set; } = string.Empty;

    public bool AutoAcknowledge { get; set; } = true;
}