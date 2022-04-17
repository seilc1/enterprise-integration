namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     Marks a method as the end of a <see cref="Message.IMessage{T}"/> flow.
    ///     <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageEndpoint.html"/>
    /// </summary>
    public class EndpointAttribute : EnterpriseIntegrationAttribute
    {
    }
}
