using EnterpriseIntegration.Channels;

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    /// Channel receives and sends an <see cref="Message.IMessage{T}"/>.
    /// <see href="https://www.enterpriseintegrationpatterns.com/patterns/messaging/MessagingAdapter.html"/>
    /// </summary>
    public class ServiceActivatorAttribute : EnterpriseIntegrationAttribute
    {
        public ServiceActivatorAttribute(string inChannelId, string outChannelId)
            : base (inChannelId)
        {
            OutChannelId = outChannelId;
        }

        /// <summary>
        /// Name of the next Channel the return value should be forwarded to.
        /// </summary>
        public ChannelId OutChannelId { get; }
    }
}
