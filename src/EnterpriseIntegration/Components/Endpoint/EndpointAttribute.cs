using EnterpriseIntegration.Channels;

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     Marks a method as the end of a <see cref="Message.IMessage{T}"/> flow.
    /// </summary>
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessageEndpoint.html"/>
    public class EndpointAttribute : EnterpriseIntegrationAttribute
    {
        public EndpointAttribute(string inChannelId) : base(inChannelId)
        {
        }
    }
}
