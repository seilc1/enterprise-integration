namespace EnterpriseIntegation.RabbitMQ;

/// <summary>
///     Configuration of RabbitMQ.
/// </summary>
public class RabbitMQSettings
{
    /// <summary>
    ///     Config Path in the appsettings, where this configuration is defined.
    /// </summary>
    public const string ConfigPath = "EnterpriseIntegration:RabbitMQ";

    public string Hostname { get; init; } = "127.0.0.1";

    public string Username { get; init; } = "guest";

    public string Password { get; init; } = "guest";
}