namespace EnterpriseIntegration.Flow.Models;

/// <summary>
///     Allows to pass additional advice to the <see cref="FlowEngine"/>.
///     
/// </summary>
/// <remarks>
///     <see cref="RoutingAdvice{T}" /> Is a default implementation.
/// </remarks>
public interface IRoutingAdvice<T>
{
    /// <summary>
    ///     Defines the <see cref="Channels.IMessagingChannel"/> where the result should be sent to.
    /// </summary>
    /// <remarks>
    ///     Superseeding the defined <see cref="Channels.IMessagingChannel"/> by the <see cref="Attributes.EnterpriseIntegrationAttribute"/>
    /// </remarks>
    public string? NextChannel { get; }

    /// <summary>
    ///     Flag if the <see cref="Message.IMessage"/> should be filtered and therefor not sent to the next <see cref="Channels.IMessagingChannel"/>.
    /// </summary>
    public bool FilterMessage { get; }

    /// <summary>
    ///     Result of the method execution, can either be <see cref="Message.IMessage"/>, <see cref="IEnumerable{T}"/> (Splitter),
    ///     or any other object to be used as <see cref="Message.IMessage.Payload"/>.
    /// </summary>
    public T Result { get; }
}
