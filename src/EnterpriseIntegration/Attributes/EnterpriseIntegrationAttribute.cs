using EnterpriseIntegration.Channels;

namespace EnterpriseIntegration.Attributes
{
    /// <summary>
    ///     Annotates an Method to be used in the enterprise integration implementation, for message handling.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class EnterpriseIntegrationAttribute : Attribute
    {
        protected EnterpriseIntegrationAttribute(ChannelId inChannelId)
        {
            InChannelId = inChannelId;
        }

        /// <summary>
        /// Name of the channel, where messages are received for this channel.
        /// </summary>
        public ChannelId InChannelId { get; }
    }
}
