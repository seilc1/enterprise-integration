namespace EnterpriseIntegration.Flow.Models;
public record RoutingAdvice<T>(T Result, string? NextChannel = null, bool FilterMessage = false) : IRoutingAdvice<T>
{
    /// <summary>
    ///     Advice to not continue further with this message.
    /// </summary>
    public static RoutingAdvice<T> FilterAdvice => new(default, FilterMessage: true);

    /// <summary>
    ///     Advice to route to a defined channel.
    /// </summary>
    public static RoutingAdvice<T> RouteAdvice(T payload, string nextChannel) => new(payload, nextChannel);
}
