namespace EnterpriseIntegation.RabbitMQ;

public class RabbitMQSettings
{
    public static string ConfigPath = "EnterpriseIntegration:RabbitMQ";

    public string Hostname { get; init; } = "127.0.0.1";

    public string Username { get; init; } = "guest";

    public string Password { get; init; } = "guest";
}